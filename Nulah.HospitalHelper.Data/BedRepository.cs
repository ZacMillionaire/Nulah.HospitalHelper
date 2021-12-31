﻿using Nulah.HospitalHelper.Core.Interfaces;
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
    public class BedRepository : IBedRepository
    {
        private readonly IDataRepository _repository;

        public BedRepository(IDataRepository sqliteDataRepository)
        {
            _repository = sqliteDataRepository;
        }

        /// <inheritdoc/>
        public List<Bed> GetBeds()
        {
            var beds = new List<Bed>();
            using (var conn = _repository.GetConnection())
            {
                conn.Open();

                var query = "SELECT * FROM [Beds]";

                using (var res = _repository.CreateCommand(query, conn))
                {
                    using (var reader = res.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                var bed = ReaderRowToBed(reader);
                                beds.Add(bed);
                            }
                        }
                    }
                }
            }
            return beds;
        }

        /// <inheritdoc/>
        public Bed? GetBedByNumber(int bedNumber)
        {
            using (var conn = _repository.GetConnection())
            {
                conn.Open();

                var query = $"SELECT [{nameof(Bed.Number)}], [{nameof(Bed.Id)}], [{nameof(Bed.BedStatus)}]" +
                    $"FROM [{nameof(Bed)}s]" +
                    $"WHERE [{nameof(Bed.Number)}] = $bedNumber";

                using (var res = _repository.CreateCommand(query, conn, new Dictionary<string, object> { { "bedNumber", bedNumber } }))
                {
                    using (var reader = res.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                return ReaderRowToBed(reader);
                            }
                        }

                        return null;
                    }
                }
            }
        }

        /// <inheritdoc/>
        public Bed? GetBedForPatient(int patientURN)
        {
            using (var conn = _repository.GetConnection())
            {
                conn.Open();

                var query = $@"SELECT [{nameof(BedPatient.BedNumber)}]
                    FROM [{nameof(BedPatient)}]
                    WHERE [{nameof(BedPatient.PatientURN)}] = $patientURN";

                using (var res = _repository.CreateCommand(query, conn, new Dictionary<string, object> { { "patientURN", patientURN } }))
                {
                    var bedNumber = res.ExecuteScalar() as long?;

                    if (bedNumber != null)
                    {
                        return GetBedByNumber(Convert.ToInt32(bedNumber));
                    }
                }
            }
            return null;
        }

        /// <inheritdoc/>
        public bool AddPatientToBed(int patientURN, int bedNumber)
        {
            using (var conn = _repository.GetConnection())
            {
                conn.Open();

                // Return false if the bed is already occupied or does not exist
                if (GetBedStatus(bedNumber) != BedStatus.Free)
                {
                    return false;
                }

                // Use INSERT OR IGNORE here so that the unique constraint on both BedNumber and PatientURN
                // prevent a patient from being assigned to a bed if they're already assigned to one
                // and visa versa prevents a bed from being assigned more than 1 patient
                var bedPatientQuery = $@"INSERT OR IGNORE INTO [{nameof(BedPatient)}] (
                        [{nameof(BedPatient.BedNumber)}], 
                        [{nameof(BedPatient.PatientURN)}]
                    )
                    VALUES (
                        $bedNumber,
                        $patientId
                    )";

                var bedPatientQueryParams = new Dictionary<string, object>
                {
                    { "bedNumber",bedNumber },
                    { "patientId", patientURN }
                };

                using (var res = _repository.CreateCommand(bedPatientQuery, conn, bedPatientQueryParams))
                {
                    var rowsInserted = res.ExecuteNonQuery();

                    return rowsInserted == 1;
                }
            }
        }

        /// <inheritdoc/>
        public bool RemovePatientFromBed(int patientURN, int bedNumber)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public BedStatus? GetBedStatus(int bedNumber)
        {
            using (var conn = _repository.GetConnection())
            {
                conn.Open();

                var bedStatusQuery = $@"SELECT 
                        [{nameof(Bed.BedStatus)}]
                    FROM 
                        [{nameof(Bed)}s]
                    WHERE 
                        [{nameof(Bed.Number)}] = $bedNumber";

                var bedStatusQueryParams = new Dictionary<string, object>
                {
                    { "bedNumber",bedNumber }
                };

                using (var res = _repository.CreateCommand(bedStatusQuery, conn, bedStatusQueryParams))
                {
                    var bedStatus = res.ExecuteScalar() as long?;
                    if (bedStatus != null)
                    {
                        return (BedStatus?)bedStatus;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Converts the current row in the given reader to a <see cref="Bed"/> object
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private Bed ReaderRowToBed(DbDataReader reader)
        {
            return new Bed
            {
                Id = Guid.Parse((string)reader[nameof(Bed.Id)]),
                BedStatus = (BedStatus)Convert.ToInt32(reader[nameof(Bed.BedStatus)]),
                Number = Convert.ToInt32(reader[nameof(Bed.Number)])
            };
        }
    }
}
