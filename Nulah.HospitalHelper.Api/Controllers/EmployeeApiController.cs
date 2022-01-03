using Microsoft.AspNetCore.Mvc;
using Nulah.HospitalHelper.Api.Models;
using Nulah.HospitalHelper.Api.Models.Employees;
using Nulah.HospitalHelper.Lib;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;
using System.Net.Mime;

namespace Nulah.HospitalHelper.Api.Controllers
{
    [ApiController]
    [Route("Employees")]
    [LazyApiAuthorise]
    public class EmployeeApiController : ControllerBase
    {
        private readonly EmployeeManager _employeeManager;
        public EmployeeApiController(EmployeeManager employeeManager)
        {
            _employeeManager = employeeManager;
        }

        [HttpPost]
        [Route("New")]
        [SwaggerResponse((int)HttpStatusCode.OK, "The newly created employee", typeof(EmployeeApiResponse), MediaTypeNames.Application.Json)]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, "Error response if any error occurs", typeof(ErrorApiResponse), MediaTypeNames.Application.Json)]
        public async Task<JsonResult> CreateEmployee(string fullName, string displayFirstName, string? displayLastName = null)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var newEmployee = _employeeManager.CreateEmployee(fullName, displayFirstName, displayLastName);

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

        [HttpPost]
        [Route("{employeeId}")]
        [SwaggerResponse((int)HttpStatusCode.OK, "The found employee", typeof(EmployeeApiResponse), MediaTypeNames.Application.Json)]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, "Error response if any error occurs", typeof(ErrorApiResponse), MediaTypeNames.Application.Json)]
        public async Task<JsonResult> GetEmployee(int employeeId)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var employee = _employeeManager.GetEmployee(employeeId);

                    if (employee == null)
                    {

                        Response.StatusCode = (int)HttpStatusCode.InternalServerError;

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
