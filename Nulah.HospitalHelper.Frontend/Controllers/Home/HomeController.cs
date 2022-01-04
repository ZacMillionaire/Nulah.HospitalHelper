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
        public HomeController(BedApiController bedApiController)
        {
            _bedApi = bedApiController;
        }

        [HttpGet]
        [Route("/")]
        public IActionResult Index()
        {
            var model = new HomeViewModel
            {
                Beds = _bedApi.GetBeds().ToList()
            };

            var a = model.Beds
                .Where(x => x.Patient != null)
                .Select(x => new
                {
                    Patient = x.Patient,
                    AdmittedCommentDate = x.Patient!.Comments
                    .FirstOrDefault(y => y.Comment == "Admitted")
                });
            //          && y.Patient.DateTimeUTC.Date == DateTime.UtcNow.Date);

            return View(model);
        }
    }
}
