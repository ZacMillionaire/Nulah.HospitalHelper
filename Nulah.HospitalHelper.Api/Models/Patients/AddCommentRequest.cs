using System.ComponentModel.DataAnnotations;

namespace Nulah.HospitalHelper.Api.Models.Patients
{

    public class AddCommentRequest
    {
        [Required]
        public string Comment { get; set; }
        [Required]
        public int PatientURN { get; set; }
        [Required]
        public int EmployeeId { get; set; }

    }
}
