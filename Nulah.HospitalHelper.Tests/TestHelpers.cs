using Microsoft.Data.Sqlite;
using Nulah.HospitalHelper.Core.Models;
using Nulah.HospitalHelper.Core.Models.Data;
using Nulah.HospitalHelper.Data;
using Nulah.HospitalHelper.Lib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nulah.HospitalHelper.Tests
{
    internal static class TestHelpers
    {
        private static SqliteDataRepository _dataRepository;

        /// <summary>
        /// Ensures that the test database exists, and that 
        /// </summary>
        internal static void ReseedDatabase()
        {
            var testDatabaseLocation = "./hospitalHelperTest.db";

            _dataRepository = new SqliteDataRepository($"Data Source={testDatabaseLocation};");

            InitialiseDatabase(_dataRepository);
            SeedDatabase(_dataRepository);
        }

        private static void SeedDatabase(SqliteDataRepository dataRepository)
        {
            using (var db = (SqliteConnection)dataRepository.GetConnection())
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

                var createBeds = new SqliteCommand(createBedsCommand, db);


                createBeds.ExecuteNonQuery();
            }
        }

        private static void InitialiseDatabase(SqliteDataRepository dataRepository)
        {

            ClearDatabase(dataRepository);

            using (var db = (SqliteConnection)dataRepository.GetConnection())
            {
                db.Open();

                var tableCommand = $"CREATE TABLE IF NOT EXISTS [{nameof(Bed)}s] (" +
                    $"[{nameof(Bed.Number)}] INTEGER PRIMARY KEY AUTOINCREMENT," +
                    $"[{nameof(Bed.Id)}] TEXT, " +
                    $"[{nameof(Bed.BedStatus)}] INTEGER," +
                    $"[{nameof(Bed.LastUpdateUTC)}] INTEGER)";

                SqliteCommand createTable = new SqliteCommand(tableCommand, db);

                createTable.ExecuteReader();

                db.Close();
            }
        }

        /// <summary>
        /// Drops all tables from the database
        /// </summary>
        /// <param name="dataRepository"></param>
        private static void ClearDatabase(SqliteDataRepository dataRepository)
        {
            // We don't simply delete the sqlite database and recreate here as tests are run in individual threads
            // so one test may have a lock on the database.
            using (var db = (SqliteConnection)dataRepository.GetConnection())
            {
                db.Open();

                var dropAllTablesCommand = $@"DROP TABLE IF EXISTS [{nameof(Bed)}s]";

                var dropAllTables = new SqliteCommand(dropAllTablesCommand, db);

                dropAllTables.ExecuteNonQuery();
            }
        }

        internal static BedManager GetBedManager()
        {
            var bedRepository = new BedRepository(_dataRepository);
            return new BedManager(bedRepository);
        }
    }
}
