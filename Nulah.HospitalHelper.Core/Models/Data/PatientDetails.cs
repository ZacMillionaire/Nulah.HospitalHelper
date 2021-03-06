using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nulah.HospitalHelper.Core.Models.Data
{
    public class PatientDetails : Patient
    {
        public List<PatientCommentFull> Comments { get; set; } = new();
        public PatientHealthDetail? HealthDetails { get; set; }
    }
}
