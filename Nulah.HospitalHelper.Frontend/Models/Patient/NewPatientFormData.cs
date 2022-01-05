using System.ComponentModel.DataAnnotations;

namespace Nulah.HospitalHelper.Frontend.Models.Patient
{
    public class NewPatientFormData
    {
        [Required]
        public string FullName { get; set; } = string.Empty;
        [Required]
        public string DisplayFirstName { get; set; } = string.Empty;
        public string? DisplayLastName { get; set; }
        [Required]
        public DateTime DateOfBirthUTC { get; set; }
    }
}
