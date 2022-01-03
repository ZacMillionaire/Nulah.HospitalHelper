using Nulah.HospitalHelper.Core.Models;

namespace Nulah.HospitalHelper.Api.Models.Patients
{
    public class PatientListApiResponse : BaseApiResponse
    {
        public List<PublicPatient> Patients { get; set; } = new();
    }
}
