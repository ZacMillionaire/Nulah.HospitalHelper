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


        public PatientComment AddCommentToPatient(string comment, int patientURN, int commentingEmployeeId)
        {
            throw new NotImplementedException();
        }

        public bool RemoveCommentFromPatient(int commentId, int patientURN)
        {
            throw new NotImplementedException();
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
    }
}
