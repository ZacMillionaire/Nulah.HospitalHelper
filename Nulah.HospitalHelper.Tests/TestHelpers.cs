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
        // To align to test data assume the dates were created in brisbane/AEST
        private static TimeZoneInfo _tz = TimeZoneInfo.GetSystemTimeZones().First(x => x.StandardName == "E. Australia Standard Time");

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
                PopulatePatientsToBeds(db);
                PopulateEmployees(db);
                PopulateComments(db);
                PopulatePatientHealthDetails(db);
            }
        }

        private static void PopulateBeds(SqliteConnection db)
        {
            var createBedsQueryText = $@"INSERT INTO [{nameof(Bed)}s] (
                    [{nameof(Bed.Id)}],
                    [{nameof(Bed.BedStatus)}]
                ) VALUES 
                    ('{Guid.NewGuid()}',{(int)BedStatus.InUse}),
                    ('{Guid.NewGuid()}',{(int)BedStatus.Free}),
                    ('{Guid.NewGuid()}',{(int)BedStatus.Free}),
                    ('{Guid.NewGuid()}',{(int)BedStatus.Free}),
                    ('{Guid.NewGuid()}',{(int)BedStatus.InUse}),
                    ('{Guid.NewGuid()}',{(int)BedStatus.InUse}),
                    ('{Guid.NewGuid()}',{(int)BedStatus.Free}),
                    ('{Guid.NewGuid()}',{(int)BedStatus.Free});";

            new SqliteCommand(createBedsQueryText, db)
                .ExecuteNonQuery();
        }

        public static void PopulatePatients(SqliteConnection db)
        {
            // DoB is assumed to be UTC, but created patients should be done in a local timezone or other
            var createPatientsQueryText = $@"INSERT INTO [{nameof(Patient)}s] (
                    [{nameof(Patient.URN)}],
                    [{nameof(Patient.Id)}],
                    [{nameof(Patient.DisplayFirstName)}],
                    [{nameof(Patient.DisplayLastName)}],
                    [{nameof(Patient.FullName)}],
                    [{nameof(Patient.DateOfBirthUTC)}] 
                ) VALUES 
                    (83524,'{Guid.NewGuid()}','John','Doe','John Doe',{CreateDateTimeForTimezone(new DateTime(1980, 1, 1), TimeZoneInfo.Utc).Ticks}),
                    (2,'{Guid.NewGuid()}','Lorna','Smith','Lorna Smith',{CreateDateTimeForTimezone(new DateTime(1995, 3, 15), TimeZoneInfo.Utc).Ticks}),
                    (3000,'{Guid.NewGuid()}','Diana','May','Diana May',{CreateDateTimeForTimezone(new DateTime(1972, 11, 23), TimeZoneInfo.Utc).Ticks})";

            new SqliteCommand(createPatientsQueryText, db)
                .ExecuteNonQuery();
        }

        private static void PopulatePatientsToBeds(SqliteConnection db)
        {
            var linkPatientsToBedsQueryText = $@"INSERT INTO [{nameof(BedPatient)}] (
                [{nameof(BedPatient.BedNumber)}],
                [{nameof(BedPatient.PatientURN)}]
            ) VALUES
                (1,83524),
                (5,2),
                (6,3000);";

            new SqliteCommand(linkPatientsToBedsQueryText, db)
                .ExecuteNonQuery();
        }

        private static void PopulateEmployees(SqliteConnection db)
        {
            var employeesQueryText = $@"INSERT INTO [{nameof(Employee)}s] (
                    [{nameof(Employee.Id)}],
                    [{nameof(Employee.DisplayFirstName)}],
                    [{nameof(Employee.DisplayLastName)}],
                    [{nameof(Employee.FullName)}]
                ) VALUES
                    ('{Guid.NewGuid()}','Kelly', 'A.', 'Kelly Anderson'),
                    ('{Guid.NewGuid()}','Mary', 'P.', 'Mary Presco')";

            new SqliteCommand(employeesQueryText, db)
                .ExecuteNonQuery();

            var employeeUserQueryText = $@"INSERT INTO [{nameof(User)}s] (
                    [{nameof(User.Id)}],
                    [{nameof(User.Salt)}],
                    [{nameof(User.PasswordHash)}]
                ) VALUES
                    (1, '2d29403ab6a17655d1141e0038cf95e4f86c6c71200028b9f4b5167795a75b78f2796a0e8d63f4da31415de8748c078e441110a7a8301f7704c2e711695f192a', '479541ed639b1fe5622d0a5523817e0ddf1ef8267f9b35b760e642f7f9f542fde117db57ee3f5babb9c7dabd43d0a966415a4b5cff5f86eafe7c123cbeb8d7e3'),
(2, '3a04d83db5c11e9d9d478cf00d8aa786d1ffaf3098701ecede98b5fc5408df2ce0a34f6902db6b1544f88bcaba5633f12515c0fd2c3898813476107df9bea5bd', 'd5d1d036ab4e662fb0e5e48480faf7b530dc188c5a326626301b462d1591a2e2769f927710802e5012e44b29843cb6403cf4c1c22fdf5dec7a33b8fd8ddc009a')";

            new SqliteCommand(employeeUserQueryText, db)
                .ExecuteNonQuery();

        }

        private static void PopulateComments(SqliteConnection db)
        {
            // Create the initial comments
            var commentsQueryText = $@"INSERT INTO [{nameof(PatientComment)}s] (
                [{nameof(PatientComment.Comment)}],
                [{nameof(PatientComment.DateTimeUTC)}]
            ) VALUES
                ('Admitted',{CreateDateTimeForTimezone(new DateTime(2020, 2, 2, 9, 50, 0), _tz).ToUniversalTime().Ticks}),
                ('Temp checked',{CreateDateTimeForTimezone(new DateTime(2020, 2, 2, 9, 55, 0), _tz).ToUniversalTime().Ticks}),
                ('Blood pressure checked',{CreateDateTimeForTimezone(new DateTime(2020, 2, 2, 10, 25, 0), _tz).ToUniversalTime().Ticks}),
                ('Discharged',{CreateDateTimeForTimezone(new DateTime(2020, 2, 2, 10, 35, 0), _tz).ToUniversalTime().Ticks}),
                ('X-Ray waiting results',{CreateDateTimeForTimezone(new DateTime(2020, 2, 2, 7, 30, 25), _tz).ToUniversalTime().Ticks}),
                ('Medication supplied',{CreateDateTimeForTimezone(new DateTime(2020, 2, 2, 9, 45, 25), _tz).ToUniversalTime().Ticks});";

            new SqliteCommand(commentsQueryText, db)
                .ExecuteNonQuery();

            var commentPatientEmployeeQueryText = $@"INSERT INTO [{nameof(CommentPatientEmployee)}] (
                    [{nameof(CommentPatientEmployee.CommentId)}],
                    [{nameof(CommentPatientEmployee.PatientURN)}],
                    [{nameof(CommentPatientEmployee.EmployeeId)}]
                ) VALUES 
                    (1,83524,1),
                    (2,83524,2),
                    (3,83524,2),
                    (4,83524,1),
                    (5,2,2),
                    (6,3000,1)";

            new SqliteCommand(commentPatientEmployeeQueryText, db)
                .ExecuteNonQuery();
        }

        private static void PopulatePatientHealthDetails(SqliteConnection db)
        {
            var patientHealthDetailsQueryText = $@"INSERT INTO [{nameof(PatientHealthDetail)}s] (
                    [{nameof(PatientHealthDetail.PatientId)}],
                    [{nameof(PatientHealthDetail.PresentingIssue)}]
                ) VALUES
                    (83524, 'Nausea, dizziness'),
                    (2, 'Broken leg'),
                    (3000, 'High fever')";

            new SqliteCommand(patientHealthDetailsQueryText, db)
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
                    [{nameof(Bed.BedStatus)}] INTEGER
                )";

                var patientTableCommand = $@"CREATE TABLE IF NOT EXISTS [{nameof(Patient)}s] (
                    [{nameof(Patient.URN)}] INTEGER PRIMARY KEY AUTOINCREMENT,
                    [{nameof(Patient.Id)}] TEXT,
                    [{nameof(Patient.DisplayFirstName)}] TEXT NOT NULL,
                    [{nameof(Patient.DisplayLastName)}] TEXT,
                    [{nameof(Patient.FullName)}] TEXT,
                    [{nameof(Patient.DateOfBirthUTC)}] INTEGER
                )";

                var bedPatientCommand = $@"CREATE TABLE IF NOT EXISTS [{nameof(BedPatient)}] (
                    [{nameof(BedPatient.BedNumber)}] INTEGER NOT NULL UNIQUE,
                    [{nameof(BedPatient.PatientURN)}] INTEGER NOT NULL UNIQUE,
                    PRIMARY KEY (
                        {nameof(BedPatient.BedNumber)},
                        {nameof(BedPatient.PatientURN)}
                    ),
                    FOREIGN KEY({nameof(BedPatient.BedNumber)}) REFERENCES [{nameof(Bed)}s]({nameof(Bed.Number)}),
                    FOREIGN KEY({nameof(BedPatient.PatientURN)}) REFERENCES [{nameof(Patient)}s]({nameof(Patient.URN)})
                )";

                var commentTableCommand = $@"CREATE TABLE IF NOT EXISTS [{nameof(PatientComment)}s] (
                    [{nameof(PatientComment.Id)}] INTEGER PRIMARY KEY AUTOINCREMENT,
                    [{nameof(PatientComment.Comment)}] TEXT,
                    [{nameof(PatientComment.DateTimeUTC)}] INTEGER
                )";

                var commentPatientEmployeeTableCommand = $@"CREATE TABLE IF NOT EXISTS [{nameof(CommentPatientEmployee)}] (
                    [{nameof(CommentPatientEmployee.CommentId)}] INTEGER NOT NULL,
                    [{nameof(CommentPatientEmployee.PatientURN)}] INTEGER NOT NULL,
                    [{nameof(CommentPatientEmployee.EmployeeId)}] INTEGER NOT NULL,
                    PRIMARY KEY (
                        {nameof(CommentPatientEmployee.CommentId)},
                        {nameof(CommentPatientEmployee.PatientURN)},
                        {nameof(CommentPatientEmployee.EmployeeId)}
                    ),
                    FOREIGN KEY({nameof(CommentPatientEmployee.CommentId)}) REFERENCES [{nameof(PatientComment)}s]({nameof(PatientComment.Id)}),
                    FOREIGN KEY({nameof(CommentPatientEmployee.PatientURN)}) REFERENCES [{nameof(Patient)}s]({nameof(Patient.URN)}),
                    FOREIGN KEY({nameof(CommentPatientEmployee.EmployeeId)}) REFERENCES [{nameof(Employee)}s]({nameof(Employee.EmployeeId)})
                )";

                var employeeTableCommand = $@"CREATE TABLE IF NOT EXISTS [{nameof(Employee)}s] (
                    [{nameof(Employee.EmployeeId)}] INTEGER PRIMARY KEY AUTOINCREMENT,
                    [{nameof(Employee.Id)}] TEXT,
                    [{nameof(Employee.DisplayFirstName)}] TEXT NOT NULL,
                    [{nameof(Employee.DisplayLastName)}] TEXT,
                    [{nameof(Employee.FullName)}] TEXT
                )";

                var patientHealthDetailsTableCommand = $@"CREATE TABLE IF NOT EXISTS [{nameof(PatientHealthDetail)}s] (
                    [{nameof(PatientHealthDetail.Id)}] INTEGER PRIMARY KEY AUTOINCREMENT,
                    [{nameof(PatientHealthDetail.PatientId)}] INTEGER,
                    [{nameof(PatientHealthDetail.PresentingIssue)}] TEXT NOT NULL,
                    FOREIGN KEY({nameof(PatientHealthDetail.PatientId)}) REFERENCES [{nameof(Patient)}s]({nameof(Patient.URN)})
                )";

                var usersTableCommand = $@"CREATE TABLE IF NOT EXISTS [{nameof(User)}s] (
                    [{nameof(User.Id)}] INTEGER PRIMARY KEY NOT NULL,
                    [{nameof(User.Salt)}] TEXT NOT NULL,
                    [{nameof(User.PasswordHash)}] TEXT NOT NULL,
                    FOREIGN KEY({nameof(User.Id)}) REFERENCES [{nameof(Employee)}s]({nameof(Employee.EmployeeId)})
                )";

                var userLoginTokenTableCommand = $@"CREATE TABLE IF NOT EXISTS [{nameof(UserLoginToken)}] (
                    [{nameof(UserLoginToken.UserId)}] INTEGER UNIQUE NOT NULL,
                    [{nameof(UserLoginToken.LoginToken)}] TEXT NOT NULL,
                    FOREIGN KEY({nameof(UserLoginToken.UserId)}) REFERENCES [{nameof(User)}s]({nameof(User.Id)})
                )";

                var patientAdmittedStatsTableCommand = $@"CREATE TABLE IF NOT EXISTS [{nameof(PatientAdmitStat)}s] (
                    [{nameof(PatientAdmitStat.DateUTC)}] INTEGER UNIQUE NOT NULL,
                    [{nameof(PatientAdmitStat.AdmittedCount)}] INTEGER NOT NULL
                )";

                var createBedTable = new SqliteCommand(bedTableCommand, db);
                var createPatientTable = new SqliteCommand(patientTableCommand, db);
                var createBedPatientTable = new SqliteCommand(bedPatientCommand, db);
                var createCommentsTable = new SqliteCommand(commentTableCommand, db);
                var createCommentPatientEmployeeTable = new SqliteCommand(commentPatientEmployeeTableCommand, db);
                var createEmployeeTable = new SqliteCommand(employeeTableCommand, db);
                var createPatientHealthDetailsTable = new SqliteCommand(patientHealthDetailsTableCommand, db);
                var createUserTable = new SqliteCommand(usersTableCommand, db);
                var createUserLoginTokenTable = new SqliteCommand(userLoginTokenTableCommand, db);
                var createPatientAdmittedStatsTable = new SqliteCommand(patientAdmittedStatsTableCommand, db);

                createBedTable.ExecuteNonQuery();
                createPatientTable.ExecuteNonQuery();
                createBedPatientTable.ExecuteNonQuery();
                createCommentsTable.ExecuteNonQuery();
                createCommentPatientEmployeeTable.ExecuteNonQuery();
                createEmployeeTable.ExecuteNonQuery();
                createPatientHealthDetailsTable.ExecuteNonQuery();
                createUserTable.ExecuteNonQuery();
                createUserLoginTokenTable.ExecuteNonQuery();
                createPatientAdmittedStatsTable.ExecuteNonQuery();

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

                var dropAllTablesCommand = $@"PRAGMA writable_schema = 1;
                delete from sqlite_master where type in ('table', 'index', 'trigger');
                PRAGMA writable_schema = 0;
                VACUUM;";

                var dropAllTables = new SqliteCommand(dropAllTablesCommand, db);

                dropAllTables.ExecuteNonQuery();
            }
        }

        internal static BedManager GetBedManager()
        {
            var bedRepository = new BedRepository(_dataRepository);
            var patientRepository = new PatientRepository(_dataRepository);
            return new BedManager(bedRepository, patientRepository);
        }

        internal static PatientManager GetPatientManager()
        {
            var patientRepository = new PatientRepository(_dataRepository);
            var bedRepository = new BedRepository(_dataRepository);
            return new PatientManager(patientRepository, bedRepository);
        }

        internal static EmployeeManager GetEmployeeManager()
        {
            var employeeRepository = new EmployeeRepository(_dataRepository);
            return new EmployeeManager(employeeRepository);
        }

        internal static UserManager GetUserManager()
        {
            var userRepository = new UserRepository(_dataRepository);
            return new UserManager(userRepository);
        }

        /// <summary>
        /// Returns a given UTC date time aligned to the given timezone (Kind will be UTC but the offset will correspond to the given timezone)
        /// </summary>
        /// <param name="utcDateTime"></param>
        /// <param name="timeZone"></param>
        /// <returns></returns>
        public static DateTime CreateDateTimeForTimezone(DateTime utcDateTime, TimeZoneInfo timeZone)
        {
            var dateTimeUnspec = DateTime.SpecifyKind(utcDateTime, DateTimeKind.Unspecified);
            return TimeZoneInfo.ConvertTimeToUtc(dateTimeUnspec, timeZone);
        }
    }
}
