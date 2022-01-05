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
        public DbCommand CreateCommand(string commandText, DbConnection connection, Dictionary<string, object>? parameters = null, DbTransaction? sqliteTransaction = null)
        {
            var command = new SqliteCommand(commandText, (SqliteConnection)connection, (SqliteTransaction?)sqliteTransaction);

            if (parameters != null)
            {
                foreach (KeyValuePair<string, object> parameter in parameters)
                {
                    command.Parameters.AddWithValue(parameter.Key, parameter.Value ?? DBNull.Value);
                }
            }

            return command;
        }

#if DEBUG
        /// <summary>
        /// Initialises the database ensuring all required tables exist.
        /// </summary>
        public void InitialiseDatabase(bool forceCreation = false)
        {

            using (var db = (SqliteConnection)GetConnection())
            {
                // If the database has already been initialised once, and we aren't forcing a creation,
                // do nothing.
                if (File.Exists(db.DataSource) == true && forceCreation == false)
                {
                    return;
                }

                db.Open();

                // If true, clear the entire database for initial data
                if (forceCreation == true)
                {

                    var dropAllTablesCommand = $@"PRAGMA writable_schema = 1;
                        delete from sqlite_master where type in ('table', 'index', 'trigger');
                        PRAGMA writable_schema = 0;
                        VACUUM;";

                    var dropAllTables = new SqliteCommand(dropAllTablesCommand, db);

                    dropAllTables.ExecuteNonQuery();
                }

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

                CreateDefaultEmployee(db);

                db.Close();
            }
        }

        private void CreateDefaultEmployee(SqliteConnection db)
        {
            var employeesQueryText = $@"INSERT INTO [{nameof(Employee)}s] (
                    [{nameof(Employee.Id)}],
                    [{nameof(Employee.DisplayFirstName)}],
                    [{nameof(Employee.DisplayLastName)}],
                    [{nameof(Employee.FullName)}]
                ) VALUES
                    ('{Guid.NewGuid()}','Pascal', 'N.', 'Pascal Nulah')";

            new SqliteCommand(employeesQueryText, db)
                .ExecuteNonQuery();

            var employeeUserQueryText = $@"INSERT INTO [{nameof(User)}s] (
                    [{nameof(User.Id)}],
                    [{nameof(User.Salt)}],
                    [{nameof(User.PasswordHash)}]
                ) VALUES
                    (1, '2d29403ab6a17655d1141e0038cf95e4f86c6c71200028b9f4b5167795a75b78f2796a0e8d63f4da31415de8748c078e441110a7a8301f7704c2e711695f192a', '479541ed639b1fe5622d0a5523817e0ddf1ef8267f9b35b760e642f7f9f542fde117db57ee3f5babb9c7dabd43d0a966415a4b5cff5f86eafe7c123cbeb8d7e3')";
            new SqliteCommand(employeeUserQueryText, db)
                .ExecuteNonQuery();
        }
#endif

    }
}