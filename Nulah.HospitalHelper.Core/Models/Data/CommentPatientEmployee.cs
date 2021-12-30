namespace Nulah.HospitalHelper.Core.Models.Data
{
    public class CommentPatientEmployee
    {
        public int CommentId { get; set; }
        /// <summary>
        /// Patient the comment belongs to
        /// </summary>
        public int PatientId { get; set; }
        /// <summary>
        /// Employee who made the comment
        /// </summary>
        public int EmployeeId { get; set; }
    }
}
