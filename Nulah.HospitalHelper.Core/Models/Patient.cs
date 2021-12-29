using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nulah.HospitalHelper.Core.Models
{
    public class Patient
    {
        public Guid Id { get; set; }
        public string DisplayFirstName { get; set; }
        public string DisplayLastName { get; set; }
        public string FullName { get; set; }
        public DateTime DateOfBirth { get; set; }

        public Guid? HealthDetails { get; set; }
    }
}
