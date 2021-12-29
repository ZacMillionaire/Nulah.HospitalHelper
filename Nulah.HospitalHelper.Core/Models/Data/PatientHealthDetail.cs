using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nulah.HospitalHelper.Core.Models.Data
{
    public class PatientHealthDetail
    {
        public Guid Id { get; set; }
        public Guid PatientId { get; set; }
        public string PresentingIssue { get; set; }
    }
}
