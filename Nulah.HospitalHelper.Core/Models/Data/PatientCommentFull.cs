using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nulah.HospitalHelper.Core.Models.Data
{
    public class PatientCommentFull : PatientComment
    {
        public string Nurse { get; set; } = string.Empty;
    }
}
