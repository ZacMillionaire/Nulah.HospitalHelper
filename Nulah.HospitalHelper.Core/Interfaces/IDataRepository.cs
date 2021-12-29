using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nulah.HospitalHelper.Core.Interfaces
{
    public interface IDataRepository
    {
        [DebuggerStepThrough]
        public DbConnection GetConnection();
        [DebuggerStepThrough]
        DbCommand CreateCommand(string commandText, DbConnection connection, Dictionary<string, object>? parameters = null);
    }
}
