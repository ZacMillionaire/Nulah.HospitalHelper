using System.ComponentModel.DataAnnotations;

namespace Nulah.HospitalHelper.Frontend.Models.Bed
{
    public class AdmitPatientToBedFormData
    {
        public int BedNumber { get; set; }
        public int PatientURN { get; set; }
        public int EmployeeId { get; set; }
        [Required]
        public string PresentingIssue { get; set; } = string.Empty;
    }
}
