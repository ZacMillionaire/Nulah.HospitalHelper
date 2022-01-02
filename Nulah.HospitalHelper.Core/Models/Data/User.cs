using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nulah.HospitalHelper.Core.Models.Data
{
    public class User
    {
        public int Id { get; set; }
        public string Salt { get; set; }
        public string PasswordHash { get; set; }
    }
}
