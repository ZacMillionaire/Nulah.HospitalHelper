using Microsoft.AspNetCore.Mvc;
using Nulah.HospitalHelper.Api.Models;
using Nulah.HospitalHelper.Api.Models.Patients;
using Nulah.HospitalHelper.Core.Models;
using Nulah.HospitalHelper.Lib;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;
using System.Net.Mime;

namespace Nulah.HospitalHelper.Api.Controllers
{
    [ApiController]
    [Route("Patients")]
    [LazyApiAuthorise]
    public class PatientApiController : ControllerBase
    {
        private readonly PatientManager _patientManager;
        private readonly EmployeeManager _employeeManager;

        public PatientApiController(PatientManager patientManager, EmployeeManager employeeManager)
        {
            _patientManager = patientManager;
            _employeeManager = employeeManager;
        }

        [HttpGet]
        [SwaggerResponse((int)HttpStatusCode.OK, "All patients in the system", typeof(PatientListApiResponse), MediaTypeNames.Application.Json)]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, "Error response if any exception is thrown", typeof(ErrorApiResponse), MediaTypeNames.Application.Json)]
        public async Task<JsonResult> GetPatients()
        {
            return await Task.Run(() =>
            {
                try
                {
                    var patients = _patientManager.GetPatients();

                    return ToJsonResult(new PatientListApiResponse
                    {
                        Patients = patients,
                        Message = $"{patients.Count} patients"
                    });
                }
                catch (Exception ex)
                {
                    return ToJsonResult(new ErrorApiResponse
                    {
                        Message = ex.Message,
                    });
                }
            });
        }

        [HttpGet]
        [Route("{patientURN}")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Short patient details", typeof(PatientApiResponse), MediaTypeNames.Application.Json)]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, "Patient does not exist", typeof(ErrorApiResponse), MediaTypeNames.Application.Json)]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, "Error response if any exception is thrown", typeof(ErrorApiResponse), MediaTypeNames.Application.Json)]
        public async Task<JsonResult> GetPatient([FromRoute] int patientURN)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var patient = _patientManager.GetPatient(patientURN);

                    if (patient == null)
                    {
                        Response.StatusCode = (int)HttpStatusCode.BadRequest;

                        return ToJsonResult(new ErrorApiResponse
                        {
                            Message = "Patient does not exist"
                        });
                    }

                    return ToJsonResult(new PatientApiResponse
                    {
                        Patient = patient
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
        [Route("New")]
        [SwaggerResponse((int)HttpStatusCode.OK, "The created patient", typeof(PatientApiResponse), MediaTypeNames.Application.Json)]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, "Patient failed to create", typeof(ErrorApiResponse), MediaTypeNames.Application.Json)]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, "Error response if any exception is thrown", typeof(ErrorApiResponse), MediaTypeNames.Application.Json)]
        public async Task<JsonResult> CreatePatient(string fullName, DateTime dateOfBirthUTC, string displayFirstName, string? displayLastName = null)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var newPatient = _patientManager.CreateNewPatient(fullName, displayFirstName, displayLastName, dateOfBirthUTC);

                    if (newPatient == null)
                    {

                        Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                        return ToJsonResult(new ErrorApiResponse
                        {
                            Message = "Failed to create patient"
                        });
                    }

                    return ToJsonResult(newPatient);
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
        [Route("{patientURN}/AddComment")]
        [SwaggerResponse((int)HttpStatusCode.OK, "The created patient", typeof(PatientCommentApiResponse), MediaTypeNames.Application.Json)]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, "No employee exists by given Id", typeof(ErrorApiResponse), MediaTypeNames.Application.Json)]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, "No patient exists by given Id", typeof(ErrorApiResponse), MediaTypeNames.Application.Json)]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, "Error response if any exception is thrown", typeof(ErrorApiResponse), MediaTypeNames.Application.Json)]
        public async Task<JsonResult> AddCommentToPatient(string comment, [FromRoute] int patientURN, int employeeId)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var employeeDetails = _employeeManager.GetEmployee(employeeId);

                    if (employeeDetails == null)
                    {
                        Response.StatusCode = (int)HttpStatusCode.BadRequest;

                        return ToJsonResult(new ErrorApiResponse
                        {
                            Message = "Employee not found"
                        });
                    }

                    var patientDetails = _patientManager.GetPatient(patientURN);

                    if (patientDetails == null)
                    {
                        Response.StatusCode = (int)HttpStatusCode.BadRequest;

                        return ToJsonResult(new ErrorApiResponse
                        {
                            Message = "Patient not found"
                        });
                    }

                    var createdComment = _patientManager.AddCommentToPatient(comment, patientURN, employeeId);

                    if (createdComment == null)
                    {
                        Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        return ToJsonResult(new ErrorApiResponse
                        {
                            Message = "Failed to create comment for patient"
                        });
                    }

                    return ToJsonResult(new PatientCommentApiResponse
                    {
                        Comment = new PublicPatientComment
                        {
                            Comment = createdComment.Comment,
                            DateTimeUTC = DateTime.UtcNow,
                            Id = employeeDetails.EmployeeId,
                            Nurse = employeeDetails.DisplayName
                        }
                    });
                }
                catch (Exception ex)
                {
                    return ToJsonResult(new ErrorApiResponse
                    {
                        Message = ex.Message,
                    });
                }
            });
        }


        /*
    public bool AddPatientToBed(int patientId, int bedNumber)
    {

        return _patientManager.AddPatientToBed(patientId, bedNumber);
    }
        */

        private JsonResult ToJsonResult(object payload)
        {
            return new JsonResult(payload);
        }
    }
}
