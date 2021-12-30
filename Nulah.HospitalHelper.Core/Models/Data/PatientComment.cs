using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nulah.HospitalHelper.Core.Models.Data
{
    public class PatientComment
    {
        public int Id { get; set; }
        public DateTime DateTimeUTC { get; set; }
        public string Comment { get; set; } = string.Empty;
    }
}
