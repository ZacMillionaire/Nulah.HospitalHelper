using Microsoft.AspNetCore.Mvc;
using Nulah.HospitalHelper.Api.Controllers;
using Nulah.HospitalHelper.Core.Models;
using Nulah.HospitalHelper.Frontend.Models;
using Nulah.HospitalHelper.Frontend.Models.Bed;

namespace Nulah.HospitalHelper.Frontend.Controllers.Bed
{
    [Route("Beds")]
    [LazyUserAuthorise]
    public class BedController : Controller
    {
        private readonly BedApiController _bedApi;
        private readonly PatientApiController _patientApi;

        public BedController(BedApiController bedApiController, PatientApiController patientApiController)
        {
            _bedApi = bedApiController;
            _patientApi = patientApiController;
        }

        [HttpGet]
        [Route("New")]
        public IActionResult NewBed()
        {
            var model = new NewBedViewModel
            {
                Bed = _bedApi.CreateBed(),
            };

            return View(model);
        }

        [HttpGet]
        [Route("{BedNumber}")]
        public IActionResult BedDetails([FromRoute] int bedNumber)
        {
            var model = new BedDetailsViewModel
            {
                Bed = _bedApi.GetBed(bedNumber)
            };

            return View(model);
        }

        [HttpGet]
        [Route("{BedNumber}/Admit")]
        public IActionResult AdmitToBed([FromRoute] int bedNumber)
        {
            var viewModel = new AdmitToBedViewModel
            {
                Bed = _bedApi.GetBed(bedNumber),
                // API should be updated to provide a means to filter out patients already assigned to a bed
                Patients = _patientApi.GetPatientList()
            };

            ViewBag.Error = TempData["Error"];

            return View(viewModel);
        }


        [HttpPost]
        [Route("{BedNumber}/Admit")]
        public IActionResult AdmitPatientToBed([FromForm] AdmitPatientToBedFormData formData)
        {
            var patientDetails = _patientApi.GetFullPatientDetails(formData.PatientURN);

            if (patientDetails != null && patientDetails.BedNumber != null)
            {
                TempData["Error"] = $"Patient is already assigned to bed: {patientDetails.BedNumber}";
                return RedirectToAction(nameof(BedController.AdmitToBed), new { bedNumber = formData.BedNumber });
            }

            var admitToBed = _patientApi.AdmitPatientToBed(formData.PatientURN, formData.BedNumber, formData.PresentingIssue, formData.EmployeeId);

            if (admitToBed == false)
            {
                // Add a super informative error here to frustrate users
                TempData["Error"] = "An error occurred when adding patient to bed";
                return RedirectToAction(nameof(BedController.AdmitToBed), new { bedNumber = formData.BedNumber });
            }

            return RedirectToAction(nameof(BedController.BedDetails), new { bedNumber = formData.BedNumber });
        }

        [HttpGet]
        [Route("{BedNumber}/AddComment")]
        public IActionResult AddComment([FromRoute] int bedNumber)
        {
            var viewModel = new AddCommentBedViewModel
            {
                Bed = _bedApi.GetBed(bedNumber)
            };

            ViewData["Error"] = TempData["Error"];

            return View(viewModel);
        }

        [HttpPost]
        [Route("{BedNumber}/AddComment")]
        public IActionResult AddCommentToPatient([FromRoute] int bedNumber, [FromForm] AddCommentToPatientInBedFormData formData)
        {
            if (User.Identity == null || User.Identity.IsAuthenticated == false)
            {
                TempData["Error"] = "Unauthenticated action";
                return RedirectToAction(nameof(AddComment));
            }

            if (string.IsNullOrWhiteSpace(formData.Comment) == true)
            {
                TempData["Error"] = "Comment cannot be empty";
                return RedirectToAction(nameof(AddComment));
            }

            var employeeId = int.Parse(User.Claims.First(x => x.Type == "EmployeeId").Value);

            var comment = _patientApi.AddComment(formData.Comment, formData.PatientURN, employeeId);

            return RedirectToAction(nameof(BedDetails), new { bedNumber = bedNumber });
        }

        [HttpGet]
        [Route("{BedNumber}/Discharge")]
        public IActionResult DischargeFromBed([FromRoute] int bedNumber)
        {
            var viewModel = new DischargeFromBedViewModel
            {
                Bed = _bedApi.GetBed(bedNumber)
            };

            return View(viewModel);
        }

        [HttpPost]
        [Route("{BedNumber}/Discharge")]
        public IActionResult DischargePatientFromBed([FromForm] DischargePatientFromBedFormData formData)
        {
            var patientDetails = _patientApi.GetFullPatientDetails(formData.PatientURN);

            if (patientDetails != null && patientDetails.BedNumber != formData.BedNumber)
            {
                TempData["Error"] = $"Cannot discharge patient not assigned to bed: {patientDetails.BedNumber}";
                return RedirectToAction(nameof(BedController.DischargeFromBed), new { bedNumber = formData.BedNumber });
            }

            var dischargePatient = _patientApi.DischargePatientFromBed(formData.PatientURN, formData.BedNumber, formData.EmployeeId);

            if (dischargePatient == false)
            {
                // Add a super informative error here to frustrate users
                TempData["Error"] = "An error occurred when discharging patient from bed";
                return RedirectToAction(nameof(BedController.DischargeFromBed), new { bedNumber = formData.BedNumber });
            }

            return RedirectToAction(nameof(BedController.BedDetails), new { bedNumber = formData.BedNumber });
        }
    }
}
