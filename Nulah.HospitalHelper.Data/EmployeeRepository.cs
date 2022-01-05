using Nulah.HospitalHelper.Core.Interfaces;
using Nulah.HospitalHelper.Core.Models.Data;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nulah.HospitalHelper.Data
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly IDataRepository _repository;

        public EmployeeRepository(IDataRepository dataRepository)
        {
            _repository = dataRepository;
        }

        /// <inheritdoc/>
        public Employee? CreateEmployee(string fullName, string displayFirstName, string? displayLastName = null)
        {
            using (var conn = _repository.GetConnection())
            {
                conn.Open();

                var employeesQueryText = $@"INSERT INTO [{nameof(Employee)}s] (
                    [{nameof(Employee.Id)}],
                    [{nameof(Employee.DisplayFirstName)}],
                    [{nameof(Employee.DisplayLastName)}],
                    [{nameof(Employee.FullName)}]
                ) VALUES
                    ('{Guid.NewGuid()}', $displayFirstName, $displayLastName, $fullName)";

                var createEmployeeParams = new Dictionary<string, object>
                {
                    { "fullName", fullName },
                    { "displayFirstName", displayFirstName },
                    { "displayLastName", displayLastName }
                };

                using (var res = _repository.CreateCommand(employeesQueryText, conn, createEmployeeParams))
                {
                    var rowsCreated = res.ExecuteNonQuery();

                    if (rowsCreated == 1)
                    {
                        var employeeInsertId = SqliteDbHelpers.GetLastInsertId(_repository, conn);

                        if (employeeInsertId != null)
                        {
                            return GetEmployee((int)employeeInsertId);
                        }
                    }
                }
            }
            return null;
        }

        /// <inheritdoc/>
        public Employee? GetEmployee(int employeeId)
        {
            using (var conn = _repository.GetConnection())
            {
                conn.Open();

                var getEmployeeQueryText = $@"SELECT 
                        [{nameof(Employee.EmployeeId)}],
                        [{nameof(Employee.Id)}],
                        [{nameof(Employee.DisplayFirstName)}],
                        [{nameof(Employee.DisplayLastName)}],
                        [{nameof(Employee.FullName)}]
                    FROM
                        [{nameof(Employee)}s]
                    WHERE
                        [{nameof(Employee.EmployeeId)}] = $employeeId";

                var employeeParams = new Dictionary<string, object>
                {
                    { "employeeId", employeeId }
                };

                using (var res = _repository.CreateCommand(getEmployeeQueryText, conn, employeeParams))
                {
                    using (var reader = res.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                return ReaderRowToEmployee(reader);
                            }
                        }
                    }
                }
            }

            return null;
        }

        /// <inheritdoc/>
        public List<Employee> GetEmployees()
        {
            using (var conn = _repository.GetConnection())
            {
                conn.Open();

                var getEmployeeQueryText = $@"SELECT 
                        [{nameof(Employee.EmployeeId)}],
                        [{nameof(Employee.Id)}],
                        [{nameof(Employee.DisplayFirstName)}],
                        [{nameof(Employee.DisplayLastName)}],
                        [{nameof(Employee.FullName)}]
                    FROM
                        [{nameof(Employee)}s]";


                using (var res = _repository.CreateCommand(getEmployeeQueryText, conn))
                {
                    using (var reader = res.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            var employees = new List<Employee>();
                            while (reader.Read())
                            {
                                employees.Add(ReaderRowToEmployee(reader));
                            }
                            return employees;
                        }
                    }
                }
            }

            return new();
        }

        private Employee ReaderRowToEmployee(DbDataReader reader)
        {
            return new Employee
            {
                Id = Guid.Parse((string)reader[nameof(Employee.Id)]),
                DisplayFirstName = (string)reader[nameof(Employee.DisplayFirstName)],
                DisplayLastName = reader[nameof(Employee.DisplayLastName)] != DBNull.Value
                    ? (string)reader[nameof(Employee.DisplayLastName)]
                    : null,
                EmployeeId = Convert.ToInt32(reader[nameof(Employee.EmployeeId)]),
                FullName = (string)reader[nameof(Employee.FullName)],
            };
        }
    }
}
