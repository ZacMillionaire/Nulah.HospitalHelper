using Nulah.HospitalHelper.Core.Interfaces;
using Nulah.HospitalHelper.Core.Models;
using Nulah.HospitalHelper.Core.Models.Data;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nulah.HospitalHelper.Data
{
    public class PatientRepository : IPatientRepository
    {
        private readonly IDataRepository _repository;

        public PatientRepository(IDataRepository dataRepository)
        {
            _repository = dataRepository;
        }

        /// <summary>
        /// Returns a patient by their URN
        /// <para>
        /// Returns null if the patient does not exist
        /// </para>
        /// </summary>
        /// <param name="patientURN"></param>
        /// <returns></returns>
        public Patient? GetPatient(int patientURN)
        {
            using (var conn = _repository.GetConnection())
            {
                conn.Open();

                var query = $@"SELECT [{nameof(Patient.URN)}],
                        [{nameof(Patient.Id)}],
                        [{nameof(Patient.DisplayFirstName)}],
                        [{nameof(Patient.DisplayLastName)}],
                        [{nameof(Patient.FullName)}],
                        [{nameof(Patient.DateOfBirthUTC)}]
                    FROM 
                        [{nameof(Patient)}s]
                    WHERE 
                        [{nameof(Patient.URN)}] = $patientId";

                using (var res = _repository.CreateCommand(query, conn, new Dictionary<string, object> { { "patientId", patientURN } }))
                {
                    using (var reader = res.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                return ReaderRowToPatient(reader);
                            }
                        }

                        return null;
                    }
                }
            }
        }

        /// <summary>
        /// Returns all top level patient details for all patients
        /// </summary>
        /// <returns></returns>
        public List<Patient> GetPatients()
        {
            var patients = new List<Patient>();
            using (var conn = _repository.GetConnection())
            {
                conn.Open();

                var query = $"SELECT * FROM [{nameof(Patient)}s]";

                using (var res = _repository.CreateCommand(query, conn))
                {
                    using (var reader = res.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                var patient = ReaderRowToPatient(reader);
                                patients.Add(patient);
                            }
                        }
                    }
                }
            }
            return patients;
        }

        /// <summary>
        /// Returns the full details for a patient by URN, including their bed details if admitted, presenting issues, and any comments associated to them
        /// </summary>
        /// <param name="patientURN"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public PatientDetails GetPatientDetails(int patientURN)
        {
            throw new NotImplementedException();
            /*
            using (var conn = _repository.GetConnection())
            {
                conn.Open();

                var query = $@"SELECT [{nameof(Patient.URN)}],
                        [{nameof(Patient.Id)}],
                        [{nameof(Patient.DisplayFirstName)}],
                        [{nameof(Patient.DisplayLastName)}],
                        [{nameof(Patient.FullName)}],
                        [{nameof(Patient.DateOfBirth)}]
                    FROM 
                        [{nameof(Patient)}s]
                    WHERE 
                        [{nameof(Patient.URN)}] = $patientId";

                using (var res = _repository.CreateCommand(query, conn, new Dictionary<string, object> { { "patientId", patientURN } }))
                {
                    using (var reader = res.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                return ReaderRowToPatient(reader);
                            }
                        }

                        return null;
                    }
                }
            }
            */
        }

        /// <summary>
        /// Adds the comment to a patient from an employee.
        /// </summary>
        /// <param name="comment"></param>
        /// <param name="patientURN"></param>
        /// <param name="commentingEmployeeId"></param>
        /// <returns></returns>
        public PatientComment? AddCommentToPatient(string comment, int patientURN, int commentingEmployeeId)
        {
            var patient = GetPatient(patientURN);

            if (patient == null)
            {
                throw new Exception($"Patient does not exist by URN:");
            }

            using (var conn = _repository.GetConnection())
            {
                conn.Open();

                using (var transaction = conn.BeginTransaction())
                {
                    var createCommentQuery = $@"INSERT INTO [{nameof(PatientComment)}s] (
                            [{nameof(PatientComment.Comment)}],
                            [{nameof(PatientComment.DateTimeUTC)}]
                        ) VALUES (
                            $comment,
                            {DateTime.UtcNow.Ticks}
                        );";

                    using (var res = _repository.CreateCommand(createCommentQuery, conn, new Dictionary<string, object> { { "comment", comment } }, transaction))
                    {
                        res.ExecuteNonQuery();
                    }

                    var commentInsertId = GetLastInsertId(conn, transaction);

                    if (commentInsertId == null)
                    {
                        transaction.Rollback();
                        throw new Exception("Comment insert failed");
                    }

                    var linkCommentQuery = $@"INSERT INTO [{nameof(CommentPatientEmployee)}] (
                            [{nameof(CommentPatientEmployee.CommentId)}],
                            [{nameof(CommentPatientEmployee.PatientId)}],
                            [{nameof(CommentPatientEmployee.EmployeeId)}]
                        ) VALUES (
                            $commentId, 
                            $patientURN, 
                            $employeeId
                        )";

                    var linkCommentQueryParams = new Dictionary<string, object>
                    {
                        { "commentId", commentInsertId },
                        { "patientURN", patientURN },
                        { "employeeId", commentingEmployeeId },
                    };

                    using (var res = _repository.CreateCommand(linkCommentQuery, conn, linkCommentQueryParams, transaction))
                    {
                        var insert = res.ExecuteNonQuery();

                        if (insert == 1)
                        {
                            transaction.Commit();

                            // Probably shouldn't call another connection in a connection but the transaction will have been commited by this point
                            return GetPatientComment((int)commentInsertId);
                        }
                    }

                    // Rollback the transaction if we fail to find the inserted comment to return, or if some other unknown branch
                    // caused a silent error
                    transaction.Rollback();
                }
            }
            return null;
        }

        /// <summary>
        /// Returns a comment by it's <paramref name="commentId"/> or null on none found
        /// </summary>
        /// <param name="commentId"></param>
        /// <returns></returns>
        public PatientComment? GetPatientComment(int commentId)
        {
            using (var conn = _repository.GetConnection())
            {
                conn.Open();

                using (var transaction = conn.BeginTransaction())
                {

                    var getCommentQuery = $@"SELECT 
                            [{nameof(PatientComment.Id)}],
                            [{nameof(PatientComment.Comment)}],
                            [{nameof(PatientComment.DateTimeUTC)}]
                        FROM
                            [{nameof(PatientComment)}s]
                        WHERE [{nameof(PatientComment.Id)}] = $commentId";

                    using (var res = _repository.CreateCommand(getCommentQuery, conn, new Dictionary<string, object> { { "commentId", commentId } }, transaction))
                    {
                        using (var reader = res.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    return ReaderRowToComment(reader);
                                }
                            }
                        }
                    }
                }
            }

            return null;
        }

        public bool RemoveCommentFromPatient(int commentId, int patientURN)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the rowid of the last INSERT command for the given <paramref name="dbConnection"/>
        /// </summary>
        /// <param name="dbConnection"></param>
        /// <returns></returns>
        private int? GetLastInsertId(DbConnection dbConnection, DbTransaction? dbTransaction = null)
        {
            using (var res = _repository.CreateCommand("SELECT last_insert_rowid();", dbConnection, dbTransaction: dbTransaction))
            {
                var lastRowId = res.ExecuteScalar() as long?;
                return lastRowId == null
                    ? null
                    : Convert.ToInt32(lastRowId);
            }
        }

        private Patient ReaderRowToPatient(DbDataReader reader)
        {
            return new Patient
            {
                Id = Guid.Parse((string)reader[nameof(Patient.Id)]),
                URN = Convert.ToInt32(reader[nameof(Patient.URN)]),
                DisplayFirstName = (string)reader[nameof(Patient.DisplayFirstName)],
                DisplayLastName = (string)reader[nameof(Patient.DisplayLastName)],
                FullName = (string)reader[nameof(Patient.FullName)],
                // DateTime is stored as a long from DateTime.UtcNow.Ticks
                DateOfBirthUTC = new DateTime((long)reader[nameof(Patient.DateOfBirthUTC)]),
            };
        }

        private PatientComment ReaderRowToComment(DbDataReader reader)
        {
            return new PatientComment
            {
                Id = Convert.ToInt32(reader[nameof(PatientComment.Id)]),
                Comment = (string)reader[nameof(PatientComment.Comment)],
                DateTimeUTC = new DateTime((long)reader[nameof(PatientComment.DateTimeUTC)])
            };
        }
    }
}
