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
    public class BedRepository : IBedRepository
    {
        private readonly IDataRepository _repository;

        public BedRepository(IDataRepository sqliteDataRepository)
        {
            _repository = sqliteDataRepository;
        }

        /// <summary>
        /// Returns all available beds in the database.
        /// <para>Will always return a <see cref="List{Bed}"/> of <see cref="Bed"/></para>
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Returns a bed by the given <paramref name="bedId"/>, or <see cref="null"/> if no bed was found.
        /// </summary>
        /// <param name="bedId"></param>
        /// <returns></returns>
        public Bed? GetBedById(int bedId)
        {
            using (var conn = _repository.GetConnection())
            {
                conn.Open();

                var query = $"SELECT [{nameof(Bed.Number)}], [{nameof(Bed.Id)}], [{nameof(Bed.BedStatus)}], [{nameof(Bed.LastUpdateUTC)}]" +
                    $"FROM [{nameof(Bed)}s]" +
                    $"WHERE [{nameof(Bed.Number)}] = $bedId";

                using (var res = _repository.CreateCommand(query, conn, new Dictionary<string, object> { { "bedId", bedId } }))
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

                var query = $@"SELECT [{nameof(BedPatient.BedId)}]
                    FROM [{nameof(BedPatient)}]
                    WHERE [{nameof(BedPatient.PatientId)}] = $patientURN";

                using (var res = _repository.CreateCommand(query, conn, new Dictionary<string, object> { { "patientURN", patientURN } }))
                {
                    var bedId = res.ExecuteScalar() as long?;

                    if (bedId != null)
                    {
                        return GetBedById(Convert.ToInt32(bedId));
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
                Number = Convert.ToInt32(reader[nameof(Bed.Number)]),
                // DateTime is stored as a long from DateTime.UtcNow.Ticks
                LastUpdateUTC = reader[nameof(Bed.LastUpdateUTC)] != DBNull.Value
                    ? new DateTime((long)reader[nameof(Bed.LastUpdateUTC)])
                    : null,
            };
        }
    }
}
