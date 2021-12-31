using Nulah.HospitalHelper.Core.Interfaces;
using Nulah.HospitalHelper.Core.Models;
using Nulah.HospitalHelper.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nulah.HospitalHelper.Lib
{
    public class BedManager
    {
        private readonly IBedRepository _bedRepository;

        public BedManager(IBedRepository bedRepository)
        {
            _bedRepository = bedRepository;
        }

        public List<PublicBed> GetBeds()
        {
            var beds = _bedRepository.GetBeds();

            return beds.Select(x => new PublicBed
            {
                BedNumber = x.Number,
                BedStatus = x.BedStatus
            })
            .ToList();
        }

        public PublicBed? GetBedById(int bedId)
        {
            var bed = _bedRepository.GetBedById(bedId);

            if (bed == null)
            {
                return null;
            }

            return new PublicBed
            {
                Id = bed.Id,
                BedNumber = bed.Number,
                BedStatus = bed.BedStatus
            };
        }
    }
}
