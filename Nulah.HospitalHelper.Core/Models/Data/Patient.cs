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
        /// <summary>
        /// DoB is stored as UTC and should be converted to the local timezone when displaying.
        /// <para>
        /// All calculations should be done on <see cref="DateTime.ToUniversalTime()"/>
        /// </para>
        /// </summary>
        public DateTime DateOfBirthUTC { get; set; }
    }
}
