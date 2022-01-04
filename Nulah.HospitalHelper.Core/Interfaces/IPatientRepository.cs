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
        /// Returns all top level patient details for all patients
        /// </summary>
        /// <returns></returns>
        public List<Patient> GetPatients();
        /// <summary>
        /// Returns a patient by their URN
        /// <para>
        /// Returns null if the patient does not exist
        /// </para>
        /// </summary>
        /// <param name="patientURN"></param>
        /// <returns></returns>
        public Patient? GetPatient(int patientURN);
        /// <summary>
        /// Returns the full details for a patient by URN, including presenting issues, and any comments associated to them.
        /// <para>
        /// Returns null if no patient exists by <paramref name="patientURN"/>
        /// </para>
        /// </summary>
        /// <param name="patientURN"></param>
        /// <returns></returns>
        public PatientDetails? GetPatientDetails(int patientURN);
        /// <summary>
        /// Adds the comment to a patient from an employee.
        /// </summary>
        /// <param name="comment"></param>
        /// <param name="patientURN"></param>
        /// <param name="commentingEmployeeId"></param>
        /// <returns></returns>
        public PatientComment? AddCommentToPatient(string comment, int patientURN, int commentingEmployeeId);
        /// <summary>
        /// Removes a comment by Id from a patient by URN
        /// </summary>
        /// <param name="commentId"></param>
        /// <param name="patientURN"></param>
        /// <returns></returns>
        public bool RemoveCommentFromPatient(int commentId, int patientURN);
        /// <summary>
        /// Returns a comment by it's <paramref name="commentId"/> or null on none found
        /// </summary>
        /// <param name="commentId"></param>
        /// <returns></returns>
        PatientComment? GetPatientComment(int commentId);
        /// <summary>
        /// Returns all comments for the given <paramref name="patientURN"/>.
        /// <para>
        /// If no comments are found, an empty <see cref="List{T}"/> of <see cref="PatientCommentFull"/> is returned
        /// </para>
        /// </summary>
        /// <param name="patientURN"></param>
        /// <returns></returns>
        List<PatientCommentFull> GetCommentsForPatient(int patientURN);
        /// <summary>
        /// Returns a patients health details by <paramref name="patientURN"/>
        /// <para>
        /// Returns null if the patient has no health detail recorded
        /// </para>
        /// </summary>
        /// <param name="patientURN"></param>
        /// <returns></returns>
        PatientHealthDetail? GetPatientHealthDetails(int patientURN);
        /// <summary>
        /// Creates a new patient with the given details.
        /// <para>
        /// Returns null if the patient was unable to be created successfully.
        /// </para>
        /// </summary>
        /// <param name="fullName"></param>
        /// <param name="displayFirstName"></param>
        /// <param name="displayLastName"></param>
        /// <param name="dateOfBirthUTC"></param>
        /// <returns></returns>
        Patient? CreatePatient(string fullName, string displayFirstName, string? displayLastName, DateTime dateOfBirthUTC);
        /// <summary>
        /// Sets a patients health details to the given <paramref name="presentingIssue"/> by <paramref name="patientURN"/>
        /// </summary>
        /// <param name="patientURN"></param>
        /// <param name="presentingIssue"></param>
        /// <returns></returns>
        PatientHealthDetail? SetHealthDetails(int patientURN, string presentingIssue);
        /// <summary>
        /// Clears a patients health details
        /// </summary>
        /// <param name="patientURN"></param>
        /// <returns>Null when patient details are cleared</returns>
        PatientHealthDetail? ClearHealthDetails(int patientURN);
    }
}
