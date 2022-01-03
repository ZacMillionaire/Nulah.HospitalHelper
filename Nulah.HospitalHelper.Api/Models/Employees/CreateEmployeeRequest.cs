using System.ComponentModel.DataAnnotations;

namespace Nulah.HospitalHelper.Api.Models.Employees
{

    public class CreateEmployeeRequest
    {
        [Required]
        public string fullName { get; set; } = string.Empty;
        [Required]
        public string displayFirstName { get; set; } = string.Empty;
        public string? displayLastName { get; set; } = null;
    }
}
