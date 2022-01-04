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
                Patients = _patientApi.GetPatientList()
            };

            return View(viewModel);
        }


        [HttpPost]
        [Route("{BedNumber}/Admit")]
        public IActionResult AdmitPatientToBed([FromForm] AdmitPatientToBedFormData formData)
        {
            var admitToBed = _patientApi.AdmitPatientToBed(formData.PatientURN, formData.BedNumber, formData.PresentingIssue, formData.EmployeeId);

            if (admitToBed == false)
            {
                // Add a super informative error here to frustrate users
                ViewBag.Error = "An error occurred when adding patient to bed";
                return RedirectToAction(nameof(BedController.AdmitToBed));
            }
            //var viewModel = new AdmitToBedViewModel
            //{
            //    Bed = _bedApi.GetBed(bedNumber),
            //    Patients = _patientApi.GetPatientList()
            //};

            return RedirectToAction();
        }

        [HttpGet]
        [Route("{BedNumber}/AddComment")]
        public IActionResult AddComment([FromRoute] int bedNumber)
        {
            var viewModel = new DischargeFromBedViewModel
            {
                Bed = _bedApi.GetBed(bedNumber)
            };

            return View(viewModel);
        }

        [HttpGet]
        [Route("{BedNumber}/Discharge")]
        public IActionResult DiscargeFromBed([FromRoute] int bedNumber)
        {
            var viewModel = new DischargeFromBedViewModel
            {
                Bed = _bedApi.GetBed(bedNumber)
            };

            return View(viewModel);
        }
    }
}
