using Nulah.HospitalHelper.Core.Models;

namespace Nulah.HospitalHelper.Frontend.Models.Patient
{
    public class PatientListViewModel
    {
        public List<PublicPatient> Patients { get; set; } = new();
    }
}
