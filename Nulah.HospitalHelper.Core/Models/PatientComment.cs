using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nulah.HospitalHelper.Core.Models
{
    public class PatientComment
    {
        public Guid Id { get; set; }
        public Guid EmployeeId { get; set; }
        public Guid BedId { get; set; }
        public DateTime DateTimeUTC { get; set; }
        public string Comment { get; set; }
    }
}
