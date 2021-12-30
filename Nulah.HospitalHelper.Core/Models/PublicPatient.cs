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
        public int URN { get; set; }
        public string DisplayURN => $"{URN}".PadLeft(7, '0');
        public string DisplayName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
    }

    public class PublicPatientDetails : PublicPatient
    {
        public Guid BedId { get; set; }
        public int BedNumber { get; set; }
        public string? PresentingIssue { get; set; }
        public List<PublicPatientComment> Comments { get; set; } = new();
    }
}
