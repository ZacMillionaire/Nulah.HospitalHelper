using System.ComponentModel.DataAnnotations;

namespace Nulah.HospitalHelper.Api.Models.Patients
{
    public class CreatePatientRequest
    {
        [Required]
        public string fullName { get; set; } = string.Empty;
        [Required]
        public DateTime dateOfBirthUTC { get; set; }
        [Required]
        public string displayFirstName { get; set; } = string.Empty;
        public string? displayLastName { get; set; } = null;
    }
}