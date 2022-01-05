using Microsoft.AspNetCore.Mvc;
using Nulah.HospitalHelper.Api.Models;
using Nulah.HospitalHelper.Api.Models.Users;
using Nulah.HospitalHelper.Lib;
using System.Net;

namespace Nulah.HospitalHelper.Api.Controllers
{
    [ApiController]
    [Route("Api/Users")]
    public class UserApiController : ControllerBase
    {
        private readonly UserManager _userManager;

        public UserApiController(UserManager userManager)
        {
            _userManager = userManager;
        }

        /// <summary>
        /// Returns a login token for the user, or null on any failed login
        /// </summary>
        /// <param name="employeeId"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public string? LoginUser(int employeeId, string password)
        {
            if (string.IsNullOrWhiteSpace(password) == false)
            {
                var logintoken = _userManager.Login(employeeId, password);

                return logintoken;
            }

            return null;
        }

        public bool LogoutUser(int employeeId, string loginToken)
        {
            if (_userManager.CheckUserLogin(employeeId, loginToken) == false)
            {
                return false;
            }

            return _userManager.Logout(employeeId);
        }

        /// <summary>
        /// Creates a new user with the given password
        /// </summary>
        /// <param name="employeeId"></param>
        /// <param name="password"></param>
        /// <returns>false if the employee doesn't exist, or the employee already has a user account</returns>
        public bool CreateNewUser(int employeeId, string password)
        {
            return _userManager.CreateUser(employeeId, password);
        }


        [HttpPost]
        [Route("Login")]
        public async Task<JsonResult> Login([FromBody] LoginRequest loginRequest)
        {
            return await Task.Run(() =>
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(loginRequest.Password) == false)
                    {
                        var logintoken = _userManager.Login(loginRequest.EmployeeId, loginRequest.Password);

                        if (logintoken != null)
                        {
                            return ToJsonResult(logintoken);
                        }
                    }
                }
                catch (Exception ex)
                {

                    Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                    return ToJsonResult(new ErrorApiResponse
                    {
                        Message = ex.Message,
                    });
                }

                Response.StatusCode = (int)HttpStatusCode.BadRequest;

                return ToJsonResult(new ErrorApiResponse
                {
                    Message = "Invalid username or password.",
                });
            });
        }

        [HttpPost]
        [Route("Logout")]
        public async Task<JsonResult> Logout([FromBody] LogoutRequest logoutRequest)
        {
            return await Task.Run(() =>
            {
                try
                {
                    if (_userManager.CheckUserLogin(logoutRequest.EmployeeId, logoutRequest.LoginToken) == false)
                    {
                        return ToJsonResult(false);
                    }

                    var logout = _userManager.Logout(logoutRequest.EmployeeId);

                    return ToJsonResult(logout);
                }
                catch (Exception ex)
                {

                    Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                    return ToJsonResult(new ErrorApiResponse
                    {
                        Message = ex.Message,
                    });
                }
            });
        }

        private JsonResult ToJsonResult(object payload)
        {
            return new JsonResult(payload);
        }
    }
}
