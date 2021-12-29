using Microsoft.AspNetCore.Mvc;
using Nulah.HospitalHelper.Core.Models;
using Nulah.HospitalHelper.Lib;

namespace Nulah.HospitalHelper.Api.Controllers
{
    [ApiController]
    [Route("Beds")]
    public class BedController : ControllerBase
    {
        private readonly BedManager _bedManager;

        public BedController(BedManager bedRepository)
        {
            _bedManager = bedRepository;
        }

        [HttpGet(Name = "./")]
        public IEnumerable<Bed> GetBeds()
        {
            return _bedManager.GetBeds();
        }
    }
}