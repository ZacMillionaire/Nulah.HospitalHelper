using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nulah.HospitalHelper.Core.Models
{
    public class PublicEmployee
    {
        public Guid Id { get; set; }
        public string DisplayName { get; set; }
        public int EmployeeId { get; set; }
        public string DisplayFirstName { get; set; }
        public string? DisplayLastName { get; set; }
        public string FullName { get; set; }
    }
}
