using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nulah.HospitalHelper.Core.Models.Data
{
    public class BedDetails : Bed
    {
        public int? PatientURN { get; set; }
    }
}
