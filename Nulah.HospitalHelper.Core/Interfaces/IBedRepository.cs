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
        Bed? GetBedById(int bedId);
        /// <summary>
        /// Returns the bed for a given patient by <paramref name="patientURN"/>, or null if they are not assigned to a bed
        /// </summary>
        /// <param name="patientURN"></param>
        /// <returns></returns>
        Bed? GetBedForPatient(int patientURN);
        List<Bed> GetBeds();
    }
}
