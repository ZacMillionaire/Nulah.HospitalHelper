using Nulah.HospitalHelper.Core.Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nulah.HospitalHelper.Core.Interfaces
{
    public interface IEmployeeRepository
    {
        /// <summary>
        /// Creates an employee with the given details
        /// </summary>
        /// <param name="fullName"></param>
        /// <param name="displayFirstName"></param>
        /// <param name="displayLastName"></param>
        /// <returns></returns>
        Employee? CreateEmployee(string fullName, string displayFirstName, string? displayLastName = null);
        /// <summary>
        /// Returns an <see cref="Employee"/> by their <see cref="Employee.EmployeeId"/>
        /// </summary>
        /// <param name="employeeId"></param>
        /// <returns></returns>
        Employee? GetEmployee(int employeeId);
    }
}
