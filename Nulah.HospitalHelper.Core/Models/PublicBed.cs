using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nulah.HospitalHelper.Core.Models
{
    public class PublicBed
    {
        public Guid Id { get; set; }
        public int BedNumber { get; set; }
        public BedStatus BedStatus { get; set; }
        public PublicPatient? Patient { get; set; }
    }
}
