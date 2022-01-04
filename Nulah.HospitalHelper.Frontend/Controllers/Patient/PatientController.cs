using Microsoft.AspNetCore.Mvc;
using Nulah.HospitalHelper.Api.Controllers;
using Nulah.HospitalHelper.Frontend.Models.Patient;

namespace Nulah.HospitalHelper.Frontend.Controllers.Patient
{
    [Route("Patient")]
    public class PatientController : Controller
    {
        private readonly PatientApiController _patientApi;
        public PatientController(PatientApiController patientApiController)
        {
            _patientApi = patientApiController;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var viewModel = new PatientListViewModel
            {
                Patients = _patientApi.GetPatientList()
            };

            return View(viewModel);
        }

        [HttpGet]
        [Route("New")]
        public IActionResult NewPatient()
        {
            if (TempData != null && TempData.ContainsKey("Errors") == true)
            {
                ViewData["Errors"] = TempData["Errors"];
            }

            return View();
        }

        [HttpPost]
        [Route("New")]
        public IActionResult NewPatientSubmit([FromForm] NewPatientFormData newPatientData)
        {
            var errors = new List<string>();
            if (string.IsNullOrWhiteSpace(newPatientData.FullName) == true)
            {
                errors.Add("Full name cannot be empty");
            }

            if (string.IsNullOrWhiteSpace(newPatientData.DisplayFirstName) == true)
            {
                errors.Add("Display first name cannot be empty");
            }

            if (errors.Count > 0)
            {
                // Not the best way to pass errors across requests
                TempData["Errors"] = errors;
                return RedirectToAction(nameof(PatientController.NewPatient));
            }

            var newPatient = _patientApi.CreateNewPatient(newPatientData.FullName, newPatientData.DisplayFirstName, newPatientData.DisplayLastName, newPatientData.DateOfBirthUTC);

            if (newPatient == null)
            {
                // Should have a more descriptive error here, but the method to create returns a patient or null
                errors.Add("Failed to create new patient");
                TempData["Errors"] = errors;
                return RedirectToAction(nameof(PatientController.NewPatient));
            }

            return RedirectToAction(nameof(PatientController.ViewPatient), new { patientURN = newPatient.URN });
        }

        [HttpGet]
        [Route("View/{patientURN}")]
        public IActionResult ViewPatient([FromRoute] int patientURN)
        {
            var viewModel = new PatientFullDetailViewModel
            {
                PatientDetails = _patientApi.GetFullPatientDetails(patientURN)
            };

            return View(viewModel);
        }
    }
}
