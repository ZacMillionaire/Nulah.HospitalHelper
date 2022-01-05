using System.ComponentModel.DataAnnotations;

namespace Nulah.HospitalHelper.Api.Models.Users
{
    public class LoginRequest
    {
        [Required]
        public int EmployeeId { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
