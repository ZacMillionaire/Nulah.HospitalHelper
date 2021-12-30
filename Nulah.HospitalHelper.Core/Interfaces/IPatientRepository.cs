using Nulah.HospitalHelper.Core.Models;
using Nulah.HospitalHelper.Core.Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nulah.HospitalHelper.Core.Interfaces
{
    public interface IPatientRepository
    {
        /// <summary>
        /// Returns a patient by their URN
        /// <para>
        /// Returns null if the patient does not exist
        /// </para>
        /// </summary>
        /// <param name="patientURN"></param>
        /// <returns></returns>
        public List<Patient> GetPatients();
        /// <summary>
        /// Returns all top level patient details for all patients
        /// </summary>
        /// <returns></returns>
        public Patient? GetPatient(int patientURN);
        /// <summary>
        /// Returns the full details for a patient by URN, including their bed details if admitted, presenting issues, and any comments associated to them
        /// </summary>
        /// <param name="patientURN"></param>
        /// <returns></returns>
        public PatientDetails GetPatientDetails(int patientURN);
        /// <summary>
        /// Adds the comment to a patient from an employee
        /// </summary>
        /// <param name="comment"></param>
        /// <param name="patientURN"></param>
        /// <param name="commentingEmployeeId"></param>
        /// <returns></returns>
        public PatientComment AddCommentToPatient(string comment, int patientURN, int commentingEmployeeId);
        /// <summary>
        /// Removes a comment by Id from a patient by URN
        /// </summary>
        /// <param name="commentId"></param>
        /// <param name="patientURN"></param>
        /// <returns></returns>
        public bool RemoveCommentFromPatient(int commentId, int patientURN);
    }
}
