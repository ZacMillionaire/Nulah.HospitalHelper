using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nulah.HospitalHelper.Api.Controllers;
using Nulah.HospitalHelper.Frontend.Models;
using Nulah.HospitalHelper.Frontend.Models.Home;

namespace Nulah.HospitalHelper.Frontend.Controllers.Home
{
    [LazyUserAuthorise]
    public class HomeController : Controller
    {
        private readonly BedApiController _bedApi;
        private readonly PatientApiController _patientApi;

        public HomeController(BedApiController bedApiController, PatientApiController patientApiController)
        {
            _bedApi = bedApiController;
            _patientApi = patientApiController;
        }

        [HttpGet]
        [Route("/")]
        public IActionResult Index()
        {
            var model = new HomeViewModel
            {
                Beds = _bedApi.GetBeds().ToList(),
                PatientsAdmittedToday = _patientApi.GetPatientsAdmittedCount(DateTime.Now)
            };

            return View(model);
        }
    }
}
