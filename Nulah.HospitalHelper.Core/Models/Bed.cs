using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nulah.HospitalHelper.Core.Models
{
    public class Bed
    {
        public Guid Id { get; set; }
        public BedStatus BedStatus { get; set; }
        public int Number { get; set; }

        public Guid? PatientId { get; set; }
    }

    public enum BedStatus
    {
        Free,
        InUse
    }
}
