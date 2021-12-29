using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nulah.HospitalHelper.Core.Models
{
    public class PublicPatient
    {
        public Guid Id { get; set; }
        public string URN { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
    }

    public class PublicPatientDetails : PublicPatient
    {
        public Guid BedId { get; set; }
        public int BedNumber { get; set; }
        public string PresentingIssue { get; set; } = string.Empty;
        public List<PublicPatientComment> Comments { get; set; } = new();
    }
}
