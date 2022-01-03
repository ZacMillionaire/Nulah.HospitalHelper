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

        /// <inheritdoc/>
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

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public PatientDetails? GetPatientDetails(int patientURN)
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
                                var patient = ReaderRowToPatient(reader);
                                var comments = GetCommentsForPatient(patient.URN);

                                var patientDetails = new PatientDetails
                                {
                                    URN = patient.URN,
                                    DateOfBirthUTC = patient.DateOfBirthUTC,
                                    DisplayFirstName = patient.DisplayFirstName,
                                    DisplayLastName = patient.DisplayLastName,
                                    FullName = patient.FullName,
                                    Id = patient.Id,
                                    Comments = comments,
                                    HealthDetails = GetPatientHealthDetails(patient.URN)
                                };

                                return patientDetails;
                            }
                        }
                    }
                }
            }

            return null;
        }

        /// <inheritdoc/>
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

                    var commentInsertId = SqliteDbHelpers.GetLastInsertId(_repository, conn, transaction);

                    if (commentInsertId == null)
                    {
                        transaction.Rollback();
                        throw new Exception("Comment insert failed");
                    }

                    var linkCommentQuery = $@"INSERT INTO [{nameof(CommentPatientEmployee)}] (
                            [{nameof(CommentPatientEmployee.CommentId)}],
                            [{nameof(CommentPatientEmployee.PatientURN)}],
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

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public List<PatientCommentFull> GetCommentsForPatient(int patientURN)
        {
            var commentsForPatient = new List<PatientCommentFull>();

            using (var conn = _repository.GetConnection())
            {
                conn.Open();

                using (var transaction = conn.BeginTransaction())
                {
                    var getCommentLinksQuery = $@"SELECT
                            [Comments].[{nameof(PatientComment.Id)}] 
                                AS [{nameof(PatientComment.Id)}],
                            [Comments].[{nameof(PatientComment.Comment)}] 
                                AS [{nameof(PatientComment.Comment)}],
                            [Comments].[{nameof(PatientComment.DateTimeUTC)}]
                                AS [{nameof(PatientComment.DateTimeUTC)}],
                            [Employees].[{nameof(Employee.DisplayFirstName)}]
                                AS [{nameof(Employee.DisplayFirstName)}],
                            [Employees].[{nameof(Employee.DisplayLastName)}]
                                AS [{nameof(Employee.DisplayLastName)}]
                        FROM [{nameof(CommentPatientEmployee)}]
	                        AS [CommentLink]
                        INNER JOIN [{nameof(PatientComment)}s]
	                        AS [Comments] 
	                        ON [Comments].[{nameof(PatientComment.Id)}] = [CommentLink].[{nameof(CommentPatientEmployee.CommentId)}]
                        LEFT JOIN [{nameof(Employee)}s] 
	                        ON [CommentLink].[{nameof(CommentPatientEmployee.EmployeeId)}] = [Employees].[{nameof(Employee.EmployeeId)}]
                        WHERE [CommentLink].[{nameof(CommentPatientEmployee.PatientURN)}] = $patientURN
                        ORDER BY [Comments].[{nameof(PatientComment.DateTimeUTC)}] ASC";

                    using (var res = _repository.CreateCommand(getCommentLinksQuery, conn, new Dictionary<string, object> { { "patientURN", patientURN } }, transaction))
                    {
                        using (var reader = res.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    commentsForPatient.Add(ReaderRowToCommentFull(reader));
                                }
                            }
                        }
                    }
                }
            }

            return commentsForPatient;
        }

        /// <inheritdoc/>
        public PatientHealthDetail? GetPatientHealthDetails(int patientURN)
        {
            using (var conn = _repository.GetConnection())
            {
                conn.Open();

                var query = $@"SELECT 
                        [{nameof(PatientHealthDetail.Id)}],
                        [{nameof(PatientHealthDetail.PatientId)}],
                        [{nameof(PatientHealthDetail.PresentingIssue)}]
                    FROM 
                        [{nameof(PatientHealthDetail)}s]
                    WHERE 
                        [{nameof(PatientHealthDetail.PatientId)}] = $patientId";

                using (var res = _repository.CreateCommand(query, conn, new Dictionary<string, object> { { "patientId", patientURN } }))
                {
                    using (var reader = res.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                return ReaderRowToPatientHealthDetail(reader);
                            }
                        }
                    }
                }
            }

            return null;
        }

        /// <inheritdoc/>
        public PatientHealthDetail? SetHealthDetails(int patientURN, string presentingIssue)
        {
            using (var conn = _repository.GetConnection())
            {
                conn.Open();

                var query = $@"INSERT OR REPLACE INTO [{nameof(PatientHealthDetail)}s] (
                        [{nameof(PatientHealthDetail.PatientId)}],
                        [{nameof(PatientHealthDetail.PresentingIssue)}]
                    ) VALUES (
                        $patientURN, $presentingIssue
                    )";

                var queryParams = new Dictionary<string, object> {
                    { "patientURN", patientURN },
                    { "presentingIssue", presentingIssue }
                };

                using (var res = _repository.CreateCommand(query, conn, queryParams))
                {
                    var rowsActioned = res.ExecuteNonQuery();

                    if (rowsActioned == 1)
                    {
                        return GetPatientHealthDetails(patientURN);
                    }
                }
            }

            return null;
        }

        public bool RemoveCommentFromPatient(int commentId, int patientURN)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Patient? CreatePatient(string fullName, string displayFirstName, string? displayLastName, DateTime dateOfBirthUTC)
        {
            using (var conn = _repository.GetConnection())
            {
                conn.Open();

                using (var transaction = conn.BeginTransaction())
                {
                    var createPatientQueryText = $@"INSERT INTO [{nameof(Patient)}s] (
                        [{nameof(Patient.Id)}],
                        [{nameof(Patient.DisplayFirstName)}],
                        [{nameof(Patient.DisplayLastName)}],
                        [{nameof(Patient.FullName)}],
                        [{nameof(Patient.DateOfBirthUTC)}] 
                    ) VALUES (
                        $patientId,
                        $displayFirstName,
                        $displayLastName,
                        $fullName,
                        $dateOfBirthUTC
                    )";

                    var createPatientParameters = new Dictionary<string, object> {
                        { "patientId", Guid.NewGuid() },
                        { "displayFirstName", displayFirstName },
                        { "displayLastName", displayLastName },
                        { "fullName", fullName },
                        { "dateOfBirthUTC", dateOfBirthUTC.Ticks },
                    };

                    using (var res = _repository.CreateCommand(createPatientQueryText, conn, createPatientParameters, transaction))
                    {
                        var rowsCreated = res.ExecuteNonQuery();

                        if (rowsCreated == 1)
                        {
                            var patientInsertId = SqliteDbHelpers.GetLastInsertId(_repository, conn, transaction);

                            if (patientInsertId != null)
                            {
                                transaction.Commit();
                                return GetPatient((int)patientInsertId);
                            }
                        }
                    }

                    // Rollback the transaction if we fail to find the inserted comment to return, or if some other unknown branch
                    // caused a silent error
                    transaction.Rollback();
                }
            }

            // Should probably throw instead if the patient failed to be created
            return null;
        }

        /// <summary>
        /// Converts the row at <paramref name="reader"/> to a <see cref="Patient"/>
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private Patient ReaderRowToPatient(DbDataReader reader)
        {
            return new Patient
            {
                Id = Guid.Parse((string)reader[nameof(Patient.Id)]),
                URN = Convert.ToInt32(reader[nameof(Patient.URN)]),
                DisplayFirstName = (string)reader[nameof(Patient.DisplayFirstName)],
                DisplayLastName = reader[nameof(Patient.DisplayLastName)] != DBNull.Value
                    ? (string)reader[nameof(Patient.DisplayLastName)]
                    : null,
                FullName = (string)reader[nameof(Patient.FullName)],
                // DateTime is stored as a long from DateTime.UtcNow.Ticks
                DateOfBirthUTC = new DateTime((long)reader[nameof(Patient.DateOfBirthUTC)]),
            };
        }

        /// <summary>
        /// Converts the row at <paramref name="reader"/> to a <see cref="PatientComment"/>
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private PatientComment ReaderRowToComment(DbDataReader reader)
        {
            return new PatientComment
            {
                Id = Convert.ToInt32(reader[nameof(PatientComment.Id)]),
                Comment = (string)reader[nameof(PatientComment.Comment)],
                DateTimeUTC = new DateTime((long)reader[nameof(PatientComment.DateTimeUTC)])
            };
        }

        /// <summary>
        /// Converts the row at <paramref name="reader"/> to a <see cref="PatientCommentFull"/>
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private PatientCommentFull ReaderRowToCommentFull(DbDataReader reader)
        {
            var baseComment = ReaderRowToComment(reader);
            var comment = new PatientCommentFull
            {
                Comment = baseComment.Comment,
                DateTimeUTC = baseComment.DateTimeUTC,
                Id = baseComment.Id
            };

            var nurseFirstName = reader[nameof(Employee.DisplayFirstName)];
            var nurseLastName = reader[nameof(Employee.DisplayLastName)];

            comment!.Nurse = nurseLastName == DBNull.Value
                ? (string)nurseFirstName
                : $"{(string)nurseFirstName} {(string)nurseLastName}";

            return comment;
        }

        /// <summary>
        /// Converts the row at <paramref name="reader"/> to a <see cref="PatientHealthDetail"/>
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private PatientHealthDetail ReaderRowToPatientHealthDetail(DbDataReader reader)
        {
            return new PatientHealthDetail
            {
                Id = Convert.ToInt32(reader[nameof(PatientHealthDetail.Id)]),
                PatientId = Convert.ToInt32(reader[nameof(PatientHealthDetail.PatientId)]),
                PresentingIssue = (string)reader[nameof(PatientHealthDetail.PresentingIssue)]
            };
        }
    }
}
