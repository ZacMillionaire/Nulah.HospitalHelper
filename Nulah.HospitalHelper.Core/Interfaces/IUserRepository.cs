using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nulah.HospitalHelper.Core.Interfaces
{
    public interface IUserRepository
    {
        /// <summary>
        /// Creates a user for the given employee
        /// </summary>
        /// <param name="employeeId"></param>
        /// <param name="password"></param>
        /// <returns>True on success, false if a user already exists for that employee</returns>
        bool CreateUser(int employeeId, string password);
        /// <summary>
        /// Logs a user in with the given password
        /// </summary>
        /// <param name="employeeId"></param>
        /// <param name="password"></param>
        /// <returns>Null on incorrect employeeId or password, login hash if successful</returns>
        string? Login(int employeeId, string password);
        /// <summary>
        /// Logs a user out by employeeId
        /// </summary>
        /// <param name="employeeId"></param>
        /// <returns></returns>
        bool Logout(int employeeId);
        /// <summary>
        /// Checks to see if the current user is logged in
        /// </summary>
        /// <param name="employeeId"></param>
        /// <param name="loginToken"></param>
        /// <returns></returns>
        bool CheckUserLogin(int employeeId, string loginToken);
    }
}
