using System.ComponentModel.DataAnnotations;

namespace Nulah.HospitalHelper.Api.Models.Patients
{
    public class AdmitDischargePatientRequest
    {
        [Required]
        public int PatientURN { get; set; }
        [Required]
        public int BedNumber { get; set; }
        [Required]
        public int EmployeeId { get; set; }
        [Required]
        public string PresentingIssue { get; set; } = string.Empty;
    }
}
