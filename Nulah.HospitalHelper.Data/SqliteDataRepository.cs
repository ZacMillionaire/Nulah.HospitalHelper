using Microsoft.Data.Sqlite;
using Nulah.HospitalHelper.Core.Interfaces;
using Nulah.HospitalHelper.Core.Models;
using Nulah.HospitalHelper.Core.Models.Data;
using System.Data.Common;
using System.Diagnostics;

namespace Nulah.HospitalHelper.Data
{
    public class SqliteDataRepository : IDataRepository
    {
        private readonly string _connectionString;

        public SqliteDataRepository(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentNullException(nameof(connectionString));
            }

            _connectionString = connectionString;
        }

        [DebuggerStepThrough]
        public DbConnection GetConnection()
        {
            return new SqliteConnection(_connectionString);
        }

        [DebuggerStepThrough]
        public DbCommand CreateCommand(string commandText, DbConnection connection, Dictionary<string, object>? parameters = null)
        {
            var command = new SqliteCommand(commandText, connection as SqliteConnection);

            if (parameters != null)
            {
                foreach (KeyValuePair<string, object> parameter in parameters)
                {
                    command.Parameters.AddWithValue(parameter.Key, parameter.Value);
                }
            }

            return command;
        }
    }
}