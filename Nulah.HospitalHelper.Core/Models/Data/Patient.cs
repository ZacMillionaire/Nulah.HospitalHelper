using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nulah.HospitalHelper.Core.Models.Data
{
    public class Patient
    {
        public Guid Id { get; set; }
        public int URN { get; set; }
        public string DisplayFirstName { get; set; } = string.Empty;
        /// <summary>
        /// Optional
        /// </summary>
        public string? DisplayLastName { get; set; } = null;
        public string FullName { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
    }
}
