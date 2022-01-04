using Microsoft.AspNetCore.Mvc;
using Nulah.HospitalHelper.Api.Controllers;
using Nulah.HospitalHelper.Frontend.Models.Bed;

namespace Nulah.HospitalHelper.Frontend.Controllers.Bed
{
    [Route("Beds")]
    public class BedController : Controller
    {
        private readonly BedApiController _bedApi;
        public BedController(BedApiController bedApiController)
        {
            _bedApi = bedApiController;
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
    }
}
