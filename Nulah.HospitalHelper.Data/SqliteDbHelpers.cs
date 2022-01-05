using Nulah.HospitalHelper.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nulah.HospitalHelper.Data
{
    public static class SqliteDbHelpers
    {
        /// <summary>
        /// Returns the rowid of the last INSERT command for the given <paramref name="dbConnection"/>
        /// </summary>
        /// <param name="dbConnection"></param>
        /// <returns></returns>
        public static int? GetLastInsertId(IDataRepository _repository, DbConnection dbConnection, DbTransaction? dbTransaction = null)
        {
            using (var res = _repository.CreateCommand("SELECT last_insert_rowid();", dbConnection, dbTransaction: dbTransaction))
            {
                var lastRowId = res.ExecuteScalar() as long?;
                return lastRowId == null
                    ? null
                    : Convert.ToInt32(lastRowId);
            }
        }

    }
}
