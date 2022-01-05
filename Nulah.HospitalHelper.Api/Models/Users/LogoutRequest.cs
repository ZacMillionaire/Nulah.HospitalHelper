using System.ComponentModel.DataAnnotations;

namespace Nulah.HospitalHelper.Api.Models.Users
{
    public class LogoutRequest
    {
        [Required]
        public int EmployeeId { get; set; }
        [Required]
        public string LoginToken { get; set; }
    }
}
