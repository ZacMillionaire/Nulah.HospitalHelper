using System.ComponentModel.DataAnnotations;

namespace Nulah.HospitalHelper.Frontend.Models.User
{
    public class LoginFormData
    {
        public int? EmployeeId { get; set; }
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public string? ReturnUrl { get; set; }
    }
}
