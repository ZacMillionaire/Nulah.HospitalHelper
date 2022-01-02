using Nulah.HospitalHelper.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nulah.HospitalHelper.Lib
{
    public class UserManager
    {
        private readonly IUserRepository _userRepository;

        public UserManager(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        /// <summary>
        /// Creates a user for the given <paramref name="employeeId"/> with the given <paramref name="password"/>
        /// </summary>
        /// <param name="employeeId"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public bool CreateUser(int employeeId, string password)
        {
            return _userRepository.CreateUser(employeeId, password);
        }

        /// <summary>
        /// Logs an employee in with the given <paramref name="employeeId"/> and <paramref name="password"/>
        /// </summary>
        /// <param name="employeeId"></param>
        /// <param name="password"></param>
        /// <returns>null if the employee does not exist or password is incorrect, login token on success</returns>
        public string? Login(int employeeId, string password)
        {
            return _userRepository.Login(employeeId, password);
        }

        public bool CheckUserLogin(int employeeId, string loginToken)
        {
            return _userRepository.CheckUserLogin(employeeId, loginToken);
        }

        /// <summary>
        /// Logs an employee out by the given <paramref name="employeeId"/>
        /// </summary>
        /// <param name="employeeId"></param>
        /// <returns></returns>
        public bool Logout(int employeeId)
        {
            return _userRepository.Logout(employeeId);
        }
    }
}
