using Microsoft.AspNetCore.Mvc;
using Nulah.HospitalHelper.Api.Models;
using Nulah.HospitalHelper.Api.Models.Employees;
using Nulah.HospitalHelper.Core.Models;
using Nulah.HospitalHelper.Lib;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;
using System.Net.Mime;

namespace Nulah.HospitalHelper.Api.Controllers
{
    [ApiController]
    [Route("Api/Employees")]
    [LazyApiAuthorise]
    public class EmployeeApiController : ControllerBase
    {
        private readonly EmployeeManager _employeeManager;
        public EmployeeApiController(EmployeeManager employeeManager)
        {
            _employeeManager = employeeManager;
        }

        /// <summary>
        /// Returns all employees
        /// </summary>
        /// <returns>Always returns a non-null list of <see cref="PublicEmployee"/></returns>
        public List<PublicEmployee> GetAllEmployees()
        {
            return _employeeManager.GetEmployees();
        }

        /// <summary>
        /// Creates a new employee
        /// </summary>
        /// <param name="fullName"></param>
        /// <param name="displayFirstName"></param>
        /// <param name="displayLastName"></param>
        /// <returns>Null on failure</returns>
        public PublicEmployee? CreateNewEmployee(string fullName, string displayFirstName, string? displayLastName = null)
        {
            var newEmployee = _employeeManager.CreateEmployee(fullName, displayFirstName, displayLastName);
            return newEmployee;
        }


        [HttpPost]
        [Route("New")]
        [SwaggerResponse((int)HttpStatusCode.OK, "The newly created employee", typeof(EmployeeApiResponse), MediaTypeNames.Application.Json)]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, "Error response if any error occurs", typeof(ErrorApiResponse), MediaTypeNames.Application.Json)]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, "Error response if any exception occurs", typeof(ErrorApiResponse), MediaTypeNames.Application.Json)]
        public async Task<JsonResult> CreateEmployee([FromBody] CreateEmployeeRequest createEmployeeRequest)
        {
            return await Task.Run(() =>
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(createEmployeeRequest.fullName)
                        || string.IsNullOrWhiteSpace(createEmployeeRequest.displayFirstName))
                    {

                        Response.StatusCode = (int)HttpStatusCode.BadRequest;

                        return ToJsonResult(new ErrorApiResponse
                        {
                            Message = "A required property was empty"
                        });
                    }

                    var newEmployee = _employeeManager.CreateEmployee(createEmployeeRequest.fullName, createEmployeeRequest.displayFirstName, createEmployeeRequest.displayLastName);

                    if (newEmployee == null)
                    {

                        Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                        return ToJsonResult(new ErrorApiResponse
                        {
                            Message = "Failed to create employee"
                        });
                    }

                    return ToJsonResult(new EmployeeApiResponse
                    {
                        Employee = newEmployee,
                        Message = "1 employee created"
                    });
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

        [HttpGet]
        [Route("{employeeId}")]
        [SwaggerResponse((int)HttpStatusCode.OK, "The found employee", typeof(EmployeeApiResponse), MediaTypeNames.Application.Json)]
        [SwaggerResponse((int)HttpStatusCode.NotFound, "Employee not found", typeof(ErrorApiResponse), MediaTypeNames.Application.Json)]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, "Error response if any error occurs", typeof(ErrorApiResponse), MediaTypeNames.Application.Json)]
        public async Task<JsonResult> GetEmployee([FromRoute] int employeeId)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var employee = _employeeManager.GetEmployee(employeeId);

                    if (employee == null)
                    {

                        Response.StatusCode = (int)HttpStatusCode.NotFound;

                        return ToJsonResult(new ErrorApiResponse
                        {
                            Message = "Employee does not exist"
                        });
                    }

                    return ToJsonResult(new EmployeeApiResponse
                    {
                        Employee = employee,
                        Message = "1 employee found"
                    });
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
