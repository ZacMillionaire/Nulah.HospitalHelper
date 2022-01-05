using Nulah.HospitalHelper.Core;
using Nulah.HospitalHelper.Core.Interfaces;
using Nulah.HospitalHelper.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nulah.HospitalHelper.Lib
{
    public class EmployeeManager
    {
        private readonly IEmployeeRepository _employeeRepository;

        public EmployeeManager(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        /// <summary>
        /// Creates an employee with the given details.
        /// </summary>
        /// <param name="fullName"></param>
        /// <param name="displayFirstName"></param>
        /// <param name="displayLastName"></param>
        /// <returns></returns>
        public PublicEmployee? CreateEmployee(string fullName, string displayFirstName, string? displayLastName = null)
        {
            var createdEmployee = _employeeRepository.CreateEmployee(fullName, displayFirstName, displayLastName);

            if (createdEmployee == null)
            {
                return null;
            }

            return new PublicEmployee
            {
                Id = createdEmployee.Id,
                EmployeeId = createdEmployee.EmployeeId,
                DisplayName = Formatters.PersonNameToDisplayFormat(createdEmployee.DisplayFirstName, createdEmployee.DisplayLastName),
                DisplayFirstName = createdEmployee.DisplayFirstName,
                DisplayLastName = createdEmployee.DisplayLastName,
                FullName = createdEmployee.FullName
            };
        }

        /// <summary>
        /// Returns the details of an employee by their <paramref name="employeeId"/>
        /// </summary>
        /// <param name="employeeId"></param>
        /// <returns></returns>
        public PublicEmployee? GetEmployee(int employeeId)
        {
            var employee = _employeeRepository.GetEmployee(employeeId);

            if (employee == null)
            {
                return null;
            }

            return new PublicEmployee
            {
                Id = employee.Id,
                EmployeeId = employee.EmployeeId,
                DisplayName = Formatters.PersonNameToDisplayFormat(employee.DisplayFirstName, employee.DisplayLastName),
                DisplayFirstName = employee.DisplayFirstName,
                DisplayLastName = employee.DisplayLastName,
                FullName = employee.FullName
            };
        }

        /// <summary>
        /// Returns all employees
        /// </summary>
        /// <returns></returns>
        public List<PublicEmployee> GetEmployees()
        {
            var employees = _employeeRepository.GetEmployees();

            return employees.Select(x => new PublicEmployee
            {
                Id = x.Id,
                EmployeeId = x.EmployeeId,
                DisplayFirstName = x.DisplayFirstName,
                DisplayLastName = x.DisplayLastName,
                DisplayName = Formatters.PersonNameToDisplayFormat(x.DisplayFirstName, x.DisplayLastName),
                FullName = x.FullName
            })
            .ToList();
        }
    }
}
