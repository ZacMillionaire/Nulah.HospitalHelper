using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nulah.HospitalHelper.Core.Models.Data
{
    public class Employee
    {
        public Guid Id { get; set; }
        public int EmployeeId { get; set; }
        public string DisplayFirstName { get; set; } = string.Empty;
        public string? DisplayLastName { get; set; }
        public string FullName { get; set; } = string.Empty;
    }
}
