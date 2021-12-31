using Nulah.HospitalHelper.Core.Models;
using Nulah.HospitalHelper.Core.Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nulah.HospitalHelper.Core.Interfaces
{
    public interface IBedRepository
    {
        /// <summary>
        /// Returns a bed by the given <paramref name="bedNumber"/>, or <see cref="null"/> if no bed was found.
        /// </summary>
        /// <param name="bedNumber"></param>
        /// <returns></returns>
        Bed? GetBedByNumber(int bedNumber);
        /// <summary>
        /// Returns the bed for a given patient by <paramref name="patientURN"/>, or null if they are not assigned to a bed
        /// </summary>
        /// <param name="patientURN"></param>
        /// <returns></returns>
        Bed? GetBedForPatient(int patientURN);
        /// <summary>
        /// Returns all available beds in the database.
        /// <para>Will always return a <see cref="List{Bed}"/> of <see cref="Bed"/></para>
        /// </summary>
        /// <returns></returns>
        List<Bed> GetBeds();
        /// <summary>
        /// Adds a patient to a bed
        /// </summary>
        /// <param name="patientURN"></param>
        /// <param name="bedNumber"></param>
        /// <returns>True if the patient was added to the bed, false if the bed was already occupied or does not exist</returns>
        bool AddPatientToBed(int patientURN, int bedNumber);
        /// <summary>
        /// Removes a patient from a bed
        /// </summary>
        /// <param name="patientURN"></param>
        /// <param name="bedNumber"></param>
        /// <returns>True if the patient was removed, false if patient was not assigned to that bed or the bed does not exist</returns>
        bool RemovePatientFromBed(int patientURN, int bedNumber);
        /// <summary>
        /// Returns the <see cref="BedStatus"/> of the given bed by Id
        /// </summary>
        /// <param name="bedNumber"></param>
        /// <returns></returns>
        BedStatus? GetBedStatus(int bedNumber);
    }
}
