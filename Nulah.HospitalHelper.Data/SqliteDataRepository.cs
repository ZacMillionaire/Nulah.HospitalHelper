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

            InitialiseDatabase();
        }

        private void InitialiseDatabase()
        {
            using (var db = (SqliteConnection)GetConnection())
            {
                db.Open();

                var tableCommand = $"CREATE TABLE IF NOT EXISTS [{nameof(Bed)}s] (" +
                    $"[{nameof(Bed.Number)}] INTEGER PRIMARY KEY AUTOINCREMENT," +
                    $"[{nameof(Bed.Id)}] TEXT, " +
                    $"[{nameof(Bed.BedStatus)}] INTEGER," +
                    $"[{nameof(Bed.LastUpdateUTC)}] INTEGER)";

                SqliteCommand createTable = new SqliteCommand(tableCommand, db);

                createTable.ExecuteReader();
            }
        }

        public void SeedDatabase()
        {
            using (var db = (SqliteConnection)GetConnection())
            {
                db.Open();

                var createBedsCommand = $@"INSERT INTO [{nameof(Bed)}s] (" +
                    $"[{nameof(Bed.Id)}]," +
                    $"[{nameof(Bed.BedStatus)}]," +
                    $"[{nameof(Bed.LastUpdateUTC)}]" +
                    $") VALUES " +
                    $"('{new Guid(1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1)}',{(int)BedStatus.Free},{DateTime.UtcNow.Ticks})," +
                    $"('{new Guid(2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2)}',{(int)BedStatus.Free},{DateTime.UtcNow.Ticks})," +
                    $"('{new Guid(3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3)}',{(int)BedStatus.Free},{DateTime.UtcNow.Ticks});";

                SqliteCommand createBeds = new SqliteCommand(createBedsCommand, db);


                createBeds.ExecuteNonQuery();
            }
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