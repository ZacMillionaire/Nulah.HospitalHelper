using Nulah.HospitalHelper.Core.Models;

namespace Nulah.HospitalHelper.Frontend.Models.Home
{
    public class HomeViewModel
    {
        public List<PublicBed> Beds { get; set; } = new();
        public int PatientsAdmittedToday { get; set; }
    }
}
