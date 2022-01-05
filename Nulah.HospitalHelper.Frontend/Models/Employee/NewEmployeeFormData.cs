using System.ComponentModel.DataAnnotations;

namespace Nulah.HospitalHelper.Frontend.Models.Employee
{
    public class NewEmployeeFormData
    {
        [Required]
        public string FullName { get; set; } = string.Empty;
        [Required]
        public string DisplayFirstName { get; set; } = string.Empty;
        public string? DisplayLastName { get; set; }
        [Required]
        public string Password { get; set; } = string.Empty;
    }
}
