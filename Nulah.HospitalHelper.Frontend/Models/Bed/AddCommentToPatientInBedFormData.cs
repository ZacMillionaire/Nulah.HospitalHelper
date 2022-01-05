using System.ComponentModel.DataAnnotations;

namespace Nulah.HospitalHelper.Frontend.Models.Bed
{
    // Contender for dumbest class name
    public class AddCommentToPatientInBedFormData
    {
        [Required]
        public string Comment { get; set; } = string.Empty;
        public int PatientURN { get; set; }
    }
}
