using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nulah.HospitalHelper.Core.Models.Data
{
    public class UserLoginToken
    {
        public int UserId { get; set; }
        public string LoginToken { get; set; }
    }
}
