using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nulah.HospitalHelper.Core.Models
{
    public class PublicPatientComment
    {
        public Guid Id { get; set; }
        public DateTime DateTimeUTC { get; set; }
        public PublicEmployee? Nurse { get; set; }
        public string Comment { get; set; } = string.Empty;
    }
}
