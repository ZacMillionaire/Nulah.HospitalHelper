using Microsoft.AspNetCore.Mvc;
using Nulah.HospitalHelper.Api.Models;
using Nulah.HospitalHelper.Core.Models;
using Nulah.HospitalHelper.Lib;

namespace Nulah.HospitalHelper.Api.Controllers
{
    [ApiController]
    [Route("Beds")]
    [LazyApiAuthorise]
    public class BedApiController : ControllerBase
    {

        private readonly BedManager _bedManager;

        public BedApiController(BedManager bedRepository)
        {
            _bedManager = bedRepository;
        }

        [HttpGet]
        public IEnumerable<PublicBed> GetBeds()
        {
            return _bedManager.GetBeds();
        }

        [HttpGet]
        [Route("{bedNumber}")]
        public PublicBed? GetBed(int bedNumber)
        {
            return _bedManager.GetBedById(bedNumber);
        }

        [HttpPost]
        [Route("New")]
        public PublicBed? CreateBed()
        {
            return _bedManager.CreateBed();
        }
    }
}