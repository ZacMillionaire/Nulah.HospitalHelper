using Nulah.HospitalHelper.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nulah.HospitalHelper.Core.Models.Data
{
    public class BedPatient
    {
        public int BedId { get; set; }
        public int PatientId { get; set; }
    }
}
