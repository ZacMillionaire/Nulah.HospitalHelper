using Nulah.HospitalHelper.Core.Models;

namespace Nulah.HospitalHelper.Frontend.Models.Bed
{
    public class AdmitToBedViewModel
    {
        public PublicBed? Bed { get; set; }
        public List<PublicPatient> Patients { get; set; } = new();
    }
}
