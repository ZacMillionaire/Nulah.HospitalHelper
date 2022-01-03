using Nulah.HospitalHelper.Core.Models;

namespace Nulah.HospitalHelper.Api.Models.Employees
{
    public class EmployeeApiResponse : BaseApiResponse
    {
        public PublicEmployee Employee { get; set; }
    }
}
