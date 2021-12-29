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

        public List<Bed> GetBeds()
        {
            return _bedRepository.GetBeds();
        }
        public Bed? GetBedById(int bedId)
        {
            return _bedRepository.GetBedById(bedId);
        }
    }
}
