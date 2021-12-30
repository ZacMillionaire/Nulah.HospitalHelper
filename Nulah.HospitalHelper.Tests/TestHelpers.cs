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

                PopulateBeds(db);
                PopulatePatients(db);
            }
        }

        private static void PopulateBeds(SqliteConnection db)
        {
            var createBedsQueryText = $@"INSERT INTO [{nameof(Bed)}s] (" +
                $"[{nameof(Bed.Id)}]," +
                $"[{nameof(Bed.BedStatus)}]," +
                $"[{nameof(Bed.LastUpdateUTC)}]" +
                $") VALUES " +
                $"('{Guid.NewGuid()}',{(int)BedStatus.Free},{DateTime.UtcNow.Ticks})," +
                $"('{Guid.NewGuid()}',{(int)BedStatus.Free},{DateTime.UtcNow.Ticks})," +
                $"('{Guid.NewGuid()}',{(int)BedStatus.Free},{DateTime.UtcNow.Ticks});";

            new SqliteCommand(createBedsQueryText, db)
                .ExecuteNonQuery();
        }

        public static void PopulatePatients(SqliteConnection db)
        {
            var createPatientsQueryText = $@"INSERT INTO [{nameof(Patient)}s] (
                    [{nameof(Patient.Id)}],
                    [{nameof(Patient.DisplayFirstName)}],
                    [{nameof(Patient.DisplayLastName)}],
                    [{nameof(Patient.FullName)}] ,
                    [{nameof(Patient.DateOfBirth)}] 
                ) VALUES 
                    ('{Guid.NewGuid()}','John','Doe','John Doe',{new DateTime(1980, 1, 1).Ticks}),
                    ('{Guid.NewGuid()}','Lorna','Smith','Lorna Smith',{new DateTime(1995, 3, 15).Ticks}),
                    ('{Guid.NewGuid()}','Diana','May','Diana May',{new DateTime(1995, 3, 15).Ticks})";

            new SqliteCommand(createPatientsQueryText, db)
                .ExecuteNonQuery();
        }

        private static void InitialiseDatabase(SqliteDataRepository dataRepository)
        {

            ClearDatabase(dataRepository);

            using (var db = (SqliteConnection)dataRepository.GetConnection())
            {
                db.Open();

                var bedTableCommand = $@"CREATE TABLE IF NOT EXISTS [{nameof(Bed)}s] (
                    [{nameof(Bed.Number)}] INTEGER PRIMARY KEY AUTOINCREMENT,
                    [{nameof(Bed.Id)}] TEXT,
                    [{nameof(Bed.BedStatus)}] INTEGER,
                    [{nameof(Bed.LastUpdateUTC)}] INTEGER)";

                var patientTableCommand = $@"CREATE TABLE IF NOT EXISTS [{nameof(Patient)}s] (
                    [{nameof(Patient.URN)}] INTEGER PRIMARY KEY AUTOINCREMENT,
                    [{nameof(Patient.Id)}] TEXT,
                    [{nameof(Patient.DisplayFirstName)}] TEXT,
                    [{nameof(Patient.DisplayLastName)}] TEXT,
                    [{nameof(Patient.FullName)}] TEXT,
                    [{nameof(Patient.DateOfBirth)}] INTEGER)";

                var createBedTable = new SqliteCommand(bedTableCommand, db);
                var createPatientTable = new SqliteCommand(patientTableCommand, db);

                createBedTable.ExecuteReader();
                createPatientTable.ExecuteReader();

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

                var dropAllTablesCommand = $@"DROP TABLE IF EXISTS [{nameof(Bed)}s];
                    DROP TABLE IF EXISTS [{nameof(Patient)}s]";

                var dropAllTables = new SqliteCommand(dropAllTablesCommand, db);

                dropAllTables.ExecuteNonQuery();
            }
        }

        internal static BedManager GetBedManager()
        {
            var bedRepository = new BedRepository(_dataRepository);
            return new BedManager(bedRepository);
        }

        internal static PatientManager GetPatientManager()
        {
            var patientRepository = new PatientRepository(_dataRepository);
            return new PatientManager(patientRepository);
        }
    }
}
