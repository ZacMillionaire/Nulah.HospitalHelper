using Nulah.HospitalHelper.Core.Models;

namespace Nulah.HospitalHelper.Api.Models.Patients
{
    public class PatientApiResponse : BaseApiResponse
    {
        public PublicPatient Patient { get; set; }
    }
}
