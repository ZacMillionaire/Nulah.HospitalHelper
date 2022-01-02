using Nulah.HospitalHelper.Core.Interfaces;
using Nulah.HospitalHelper.Core.Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Nulah.HospitalHelper.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly IDataRepository _repository;

        public UserRepository(IDataRepository repository)
        {
            _repository = repository;
        }

        /// <inheritdoc/>
        public bool CreateUser(int employeeId, string password)
        {
            if (EmployeeExistsById(employeeId) == false
                || UserExistsById(employeeId) == true
                || string.IsNullOrWhiteSpace(password) == true)
            {
                return false;
            }

            using (var conn = _repository.GetConnection())
            {
                conn.Open();

                var employeesQueryText = $@"INSERT INTO [{nameof(User)}s] (
                    [{nameof(User.Id)}],
                    [{nameof(User.Salt)}],
                    [{nameof(User.PasswordHash)}]
                ) VALUES
                    ($employeeId, $salt, $passwordHash)";

                var salt = GenerateSalt();

                var createEmployeeParams = new Dictionary<string, object>
                {
                    { "employeeId", employeeId },
                    { "salt", salt },
                    { "passwordHash", HashPassword(salt,password) }
                };

                using (var res = _repository.CreateCommand(employeesQueryText, conn, createEmployeeParams))
                {
                    var rowsCreated = res.ExecuteNonQuery();

                    return rowsCreated == 1;
                }
            }
        }

        /// <inheritdoc/>
        public string? Login(int employeeId, string password)
        {
            if (EmployeeExistsById(employeeId) == false
                || UserExistsById(employeeId) == false
                || UserHasLoginToken(employeeId) == true
                || string.IsNullOrWhiteSpace(password) == true)
            {
                return null;
            }

            using (var conn = _repository.GetConnection())
            {
                conn.Open();

                var getSaltAndPasswordQuery = $@"SELECT 
                        [{nameof(User.Salt)}], 
                        [{nameof(User.PasswordHash)}]
                    FROM
                        [{nameof(User)}s] 
                    WHERE
                        [{nameof(User.Id)}] = $employeeId";

                var getSaltAndPasswordParams = new Dictionary<string, object>
                {
                    { "employeeId", employeeId }
                };


                using (var res = _repository.CreateCommand(getSaltAndPasswordQuery, conn, getSaltAndPasswordParams))
                {
                    using (var reader = res.ExecuteReader())
                    {
                        if (reader.HasRows == true)
                        {
                            while (reader.Read())
                            {
                                var storedSalt = (string)reader[nameof(User.Salt)];
                                var storedPasswordHash = (string)reader[nameof(User.PasswordHash)];

                                var userPasswordHash = HashPassword(storedSalt, password);

                                if (storedPasswordHash != userPasswordHash)
                                {
                                    return null;
                                }
                            }
                        }
                    }
                }

                return CreateLoginToken(employeeId);
            }

            return null;
        }

        /// <inheritdoc/>
        public bool CheckUserLogin(int employeeId, string loginToken)
        {
            return CheckLoginToken(employeeId, loginToken);
        }

        /// <inheritdoc/>
        public bool Logout(int employeeId)
        {
            if (EmployeeExistsById(employeeId) == false
                || UserExistsById(employeeId) == false
                || UserHasLoginToken(employeeId) == false)
            {
                return false;
            }

            using (var conn = _repository.GetConnection())
            {
                conn.Open();

                var logoutQuery = $@"DELETE FROM
                        [{nameof(UserLoginToken)}]
                    WHERE
                        [{nameof(UserLoginToken.UserId)}] = $employeeId";


                var logoutParams = new Dictionary<string, object>
                {
                    { "employeeId", employeeId }
                };

                using (var res = _repository.CreateCommand(logoutQuery, conn, logoutParams))
                {
                    var rowsCreated = res.ExecuteNonQuery();

                    if (rowsCreated == 1)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Returns true if the employee exists by the given Id
        /// </summary>
        /// <param name="employeeId"></param>
        /// <returns></returns>
        private bool EmployeeExistsById(int employeeId)
        {
            using (var conn = _repository.GetConnection())
            {
                conn.Open();

                var employeeExistsQuery = $@"SELECT 1
                    FROM
                        [{nameof(Employee)}s]
                    WHERE
                        [{nameof(Employee.EmployeeId)}] = $employeeId";

                var employeeExistsParams = new Dictionary<string, object>
                {
                    { "employeeId", employeeId }
                };

                using (var res = _repository.CreateCommand(employeeExistsQuery, conn, employeeExistsParams))
                {
                    var employeeExists = res.ExecuteScalar() as long?;

                    return employeeExists != null && employeeExists == 1;
                }
            }
        }

        /// <summary>
        /// Returns true if a user exists by the given Id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        private bool UserExistsById(int userId)
        {
            using (var conn = _repository.GetConnection())
            {
                conn.Open();

                var userExistsQuery = $@"SELECT 1
                    FROM
                        [{nameof(User)}s]
                    WHERE
                        [{nameof(User.Id)}] = $userId";

                var userExistsParams = new Dictionary<string, object>
                {
                    { "userId", userId }
                };

                using (var res = _repository.CreateCommand(userExistsQuery, conn, userExistsParams))
                {
                    var userExists = res.ExecuteScalar() as long?;

                    return userExists != null && userExists == 1;
                }
            }
        }

        /// <summary>
        /// Returns true if the given userId has a login token associated to it
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        private bool UserHasLoginToken(int userId)
        {
            using (var conn = _repository.GetConnection())
            {
                conn.Open();

                var userLoginTokenExistsQuery = $@"SELECT 1
                    FROM
                        [{nameof(UserLoginToken)}]
                    WHERE
                        [{nameof(UserLoginToken.UserId)}] = $userId";

                var userLoginTokenParams = new Dictionary<string, object>
                {
                    { "userId", userId }
                };

                using (var res = _repository.CreateCommand(userLoginTokenExistsQuery, conn, userLoginTokenParams))
                {
                    var userLoginTokenExists = res.ExecuteScalar() as long?;

                    return userLoginTokenExists != null && userLoginTokenExists == 1;
                }
            }
        }

        /// <summary>
        /// Naively generates salt by returning the hash of a guid
        /// </summary>
        /// <returns></returns>
        private string GenerateSalt()
        {
            return HashString(Guid.NewGuid().ToString());
        }

        /// <summary>
        /// Naively generates a hash of a string
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private string HashString(string input)
        {
            var data = SHA512.Create()
                .ComputeHash(Encoding.UTF8.GetBytes(input));

            var sb = new StringBuilder();

            for (int i = 0; i < data.Length; i++)
            {
                sb.Append(data[i].ToString("x2"));
            }

            return sb.ToString();
        }

        /// <summary>
        /// Naively returns the hash of a plain text password with given salt
        /// </summary>
        /// <param name="salt"></param>
        /// <param name="passwordRaw"></param>
        /// <returns></returns>
        private string HashPassword(string salt, string passwordRaw)
        {
            var sb = new StringBuilder();

            foreach (char c in salt)
            {
                sb.Append(c);
            }

            foreach (char c in passwordRaw)
            {
                sb.Append(c);
            }

            return HashString(sb.ToString());
        }

        /// <summary>
        /// Creates a token to indicate a user is logged in against the given employee Id
        /// </summary>
        /// <param name="employeeId"></param>
        /// <returns></returns>
        private string? CreateLoginToken(int employeeId)
        {
            if (EmployeeExistsById(employeeId) == false)
            {
                return null;
            }

            using (var conn = _repository.GetConnection())
            {
                conn.Open();

                var createUserLoginTokenQuery = $@"INSERT INTO [{nameof(UserLoginToken)}] (
                    [{nameof(UserLoginToken.UserId)}],
                    [{nameof(UserLoginToken.LoginToken)}]
                ) VALUES
                    ($userId, $loginToken)";

                var loginToken = HashString(Guid.NewGuid().ToString());

                var createUserLoginTokenParams = new Dictionary<string, object>
                {
                    { "userId", employeeId },
                    { "loginToken", loginToken },
                };

                using (var res = _repository.CreateCommand(createUserLoginTokenQuery, conn, createUserLoginTokenParams))
                {
                    var rowsCreated = res.ExecuteNonQuery();

                    if (rowsCreated == 1)
                    {
                        return loginToken;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Returns true if the given employee Id exists with the given login token
        /// </summary>
        /// <param name="employeeId"></param>
        /// <param name="loginToken"></param>
        /// <returns></returns>
        private bool CheckLoginToken(int employeeId, string loginToken)
        {
            if (EmployeeExistsById(employeeId) == false
                || UserExistsById(employeeId) == false
                || UserHasLoginToken(employeeId) == false
                || string.IsNullOrWhiteSpace(loginToken) == true)
            {
                return false;
            }

            using (var conn = _repository.GetConnection())
            {
                conn.Open();

                var checkLoginQuery = $@"SELECT 1
                    FROM
                        [{nameof(UserLoginToken)}]
                    WHERE
                        [{nameof(UserLoginToken.UserId)}] = $employeeId
                    AND
                        [{nameof(UserLoginToken.LoginToken)}] = $loginToken";


                var checkLoginParams = new Dictionary<string, object>
                {
                    { "employeeId", employeeId },
                    { "loginToken", loginToken }
                };

                using (var res = _repository.CreateCommand(checkLoginQuery, conn, checkLoginParams))
                {
                    var rowsCreated = res.ExecuteScalar() as long?;

                    return rowsCreated != null && rowsCreated == 1;
                }
            }
        }
    }
}
