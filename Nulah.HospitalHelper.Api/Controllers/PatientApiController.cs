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
        private readonly BedManager _bedManager;

        public PatientApiController(PatientManager patientManager, EmployeeManager employeeManager, BedManager bedManager)
        {
            _patientManager = patientManager;
            _employeeManager = employeeManager;
            _bedManager = bedManager;
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
        [SwaggerResponse((int)HttpStatusCode.NotFound, "Patient does not exist", typeof(ErrorApiResponse), MediaTypeNames.Application.Json)]
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
                        Response.StatusCode = (int)HttpStatusCode.NotFound;

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

        [HttpGet]
        [Route("{patientURN}/Details")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Full patient details", typeof(PatientDetailsApiResponse), MediaTypeNames.Application.Json)]
        [SwaggerResponse((int)HttpStatusCode.NotFound, "Patient does not exist", typeof(ErrorApiResponse), MediaTypeNames.Application.Json)]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, "Error response if any exception is thrown", typeof(ErrorApiResponse), MediaTypeNames.Application.Json)]
        public async Task<JsonResult> GetPatientDetails([FromRoute] int patientURN)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var patient = _patientManager.GetPatientDetails(patientURN);

                    if (patient == null)
                    {
                        Response.StatusCode = (int)HttpStatusCode.NotFound;

                        return ToJsonResult(new ErrorApiResponse
                        {
                            Message = "Patient does not exist"
                        });
                    }

                    return ToJsonResult(new PatientDetailsApiResponse
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
        [SwaggerResponse((int)HttpStatusCode.BadRequest, "Required value was missing", typeof(ErrorApiResponse), MediaTypeNames.Application.Json)]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, "Patient failed to create", typeof(ErrorApiResponse), MediaTypeNames.Application.Json)]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, "Error response if any exception is thrown", typeof(ErrorApiResponse), MediaTypeNames.Application.Json)]
        public async Task<JsonResult> CreatePatient([FromBody] CreatePatientRequest createPatientRequest)
        {
            return await Task.Run(() =>
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(createPatientRequest.fullName)
                        || string.IsNullOrWhiteSpace(createPatientRequest.displayFirstName))
                    {
                        Response.StatusCode = (int)HttpStatusCode.BadRequest;

                        return ToJsonResult(new ErrorApiResponse
                        {
                            Message = "A required value was null"
                        });
                    }

                    var newPatient = _patientManager.CreateNewPatient(createPatientRequest.fullName, createPatientRequest.displayFirstName, createPatientRequest.displayLastName, createPatientRequest.dateOfBirthUTC);

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
        [Route("AddComment")]
        [SwaggerResponse((int)HttpStatusCode.OK, "The created patient", typeof(PatientCommentApiResponse), MediaTypeNames.Application.Json)]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, "No employee exists by given Id", typeof(ErrorApiResponse), MediaTypeNames.Application.Json)]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, "No patient exists by given Id", typeof(ErrorApiResponse), MediaTypeNames.Application.Json)]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, "Error response if any exception is thrown", typeof(ErrorApiResponse), MediaTypeNames.Application.Json)]
        public async Task<JsonResult> AddCommentToPatient([FromBody] AddCommentRequest addCommentRequest)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var employeeDetails = _employeeManager.GetEmployee(addCommentRequest.EmployeeId);

                    if (employeeDetails == null)
                    {
                        Response.StatusCode = (int)HttpStatusCode.BadRequest;

                        return ToJsonResult(new ErrorApiResponse
                        {
                            Message = "Employee not found"
                        });
                    }

                    var patientDetails = _patientManager.GetPatient(addCommentRequest.PatientURN);

                    if (patientDetails == null)
                    {
                        Response.StatusCode = (int)HttpStatusCode.BadRequest;

                        return ToJsonResult(new ErrorApiResponse
                        {
                            Message = "Patient not found"
                        });
                    }

                    var createdComment = _patientManager.AddCommentToPatient(addCommentRequest.Comment, addCommentRequest.PatientURN, addCommentRequest.EmployeeId);

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


        [HttpPost]
        [Route("Admit")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Patient was added to bed", typeof(bool), MediaTypeNames.Application.Json)]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, "Error response if any error occurs", typeof(ErrorApiResponse), MediaTypeNames.Application.Json)]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, "Error response if any exception occurs", typeof(ErrorApiResponse), MediaTypeNames.Application.Json)]
        public async Task<JsonResult> AddPatientToBedByEmployee([FromBody] AdmitDischargePatientRequest admitDischargePatientRequest)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var bed = _bedManager.GetBedById(admitDischargePatientRequest.BedNumber);

                    if (bed == null)
                    {
                        Response.StatusCode = (int)HttpStatusCode.BadRequest;
                        return ToJsonResult(new ErrorApiResponse
                        {
                            Message = "Bed does not exist",
                        });
                    }
                    else if (bed.BedStatus != BedStatus.Free)
                    {
                        Response.StatusCode = (int)HttpStatusCode.BadRequest;
                        return ToJsonResult(new ErrorApiResponse
                        {
                            Message = "Bed is not free",
                        });
                    }

                    var employee = _employeeManager.GetEmployee(admitDischargePatientRequest.EmployeeId);

                    if (employee == null)
                    {
                        Response.StatusCode = (int)HttpStatusCode.BadRequest;
                        return ToJsonResult(new ErrorApiResponse
                        {
                            Message = "Employee does not exist",
                        });
                    }

                    var patientAddedToBed = _patientManager.AddPatientToBed(admitDischargePatientRequest.PatientURN, admitDischargePatientRequest.BedNumber, admitDischargePatientRequest.PresentingIssue);

                    if (patientAddedToBed == true)
                    {
                        // Create a comment for the patient to indicate they were admitted
                        var admittedComment = _patientManager.AddCommentToPatient("Admitted", admitDischargePatientRequest.PatientURN, admitDischargePatientRequest.EmployeeId);

                        if (admittedComment == null)
                        {
                            Response.StatusCode = (int)HttpStatusCode.BadRequest;
                            return ToJsonResult(new ErrorApiResponse
                            {
                                Message = "Failed to create activity comment to patient",
                            });
                        }
                    }

                    return ToJsonResult(patientAddedToBed);
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
        [Route("Discharge")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Patient was removed from bed", typeof(bool), MediaTypeNames.Application.Json)]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, "Error response if any error occurs", typeof(ErrorApiResponse), MediaTypeNames.Application.Json)]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, "Error response if any exception occurs", typeof(ErrorApiResponse), MediaTypeNames.Application.Json)]
        public async Task<JsonResult> RemovePatientFromBedByEmployee([FromBody] AdmitDischargePatientRequest admitDischargePatientRequest)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var bed = _bedManager.GetBedById(admitDischargePatientRequest.BedNumber);

                    if (bed == null)
                    {
                        Response.StatusCode = (int)HttpStatusCode.BadRequest;
                        return ToJsonResult(new ErrorApiResponse
                        {
                            Message = "Bed does not exist",
                        });
                    }
                    else if (bed.BedStatus != BedStatus.InUse)
                    {
                        Response.StatusCode = (int)HttpStatusCode.BadRequest;
                        return ToJsonResult(new ErrorApiResponse
                        {
                            Message = "Bed is not in use",
                        });
                    }
                    else if (bed.BedStatus == BedStatus.InUse)
                    {

                        if (bed.Patient == null)
                        {
                            Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                            return ToJsonResult(new ErrorApiResponse
                            {
                                Message = "Bed is in invalid state",
                            });
                        }

                        if (bed.Patient.URN != admitDischargePatientRequest.PatientURN)
                        {
                            Response.StatusCode = (int)HttpStatusCode.BadRequest;
                            return ToJsonResult(new ErrorApiResponse
                            {
                                Message = "Patient is not admitted to bed",
                            });
                        }
                    }

                    var employee = _employeeManager.GetEmployee(admitDischargePatientRequest.EmployeeId);

                    if (employee == null)
                    {
                        Response.StatusCode = (int)HttpStatusCode.BadRequest;
                        return ToJsonResult(new ErrorApiResponse
                        {
                            Message = "Employee does not exist",
                        });
                    }

                    var patientRemovedFromBed = _patientManager.RemovePatientFromBed(admitDischargePatientRequest.PatientURN, admitDischargePatientRequest.BedNumber);

                    if (patientRemovedFromBed == true)
                    {
                        // Create a comment for the patient to indicate they were discharged
                        var admittedComment = _patientManager.AddCommentToPatient("Discharged", admitDischargePatientRequest.PatientURN, admitDischargePatientRequest.EmployeeId);

                        if (admittedComment == null)
                        {
                            Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                            return ToJsonResult(new ErrorApiResponse
                            {
                                Message = "Failed to create activity comment to patient",
                            });
                        }
                    }

                    return ToJsonResult(patientRemovedFromBed);
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
