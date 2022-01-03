using Nulah.HospitalHelper.Core.Models;

namespace Nulah.HospitalHelper.Api.Models.Patients
{
    public class PatientDetailsApiResponse : BaseApiResponse
    {
        public PublicPatientDetails? Patient { get; set; }
    }
}
