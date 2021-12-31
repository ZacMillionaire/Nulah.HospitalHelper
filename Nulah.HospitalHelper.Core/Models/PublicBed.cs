using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nulah.HospitalHelper.Core.Models
{
    public class PublicBed
    {
        public Guid Id { get; set; }
        public int BedNumber { get; set; }
        public BedStatus BedStatus { get; set; }
        public PublicPatientDetails? Patient { get; set; }
        public string? LastComment
        {
            get
            {
                if (Patient != null && Patient.Comments.Count != 0)
                {
                    return Patient.Comments.Last().Comment;
                }
                return null;
            }
        }
        public DateTime? LastUpdatedUTC
        {
            get
            {
                if (Patient != null && Patient.Comments.Count != 0)
                {
                    return Patient.Comments.Last().DateTimeUTC;
                }
                return null;
            }
        }
    }
}
