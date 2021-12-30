using Nulah.HospitalHelper.Core.Interfaces;
using Nulah.HospitalHelper.Core.Models;
using Nulah.HospitalHelper.Core.Models.Data;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nulah.HospitalHelper.Lib
{
    public class PatientManager
    {
        private readonly IPatientRepository _patientRepository;

        public PatientManager(IPatientRepository patientRepository)
        {
            _patientRepository = patientRepository;
        }

        /// <summary>
        /// Returns all patients in the system
        /// </summary>
        /// <returns></returns>
        public List<PublicPatient> GetPatients()
        {
            var patients = _patientRepository.GetPatients();

            return patients.Select(x => new PublicPatient
            {
                Id = x.Id,
                DateOfBirth = x.DateOfBirthUTC,
                DisplayName = $"{x.DisplayFirstName} {x.DisplayLastName}",
                URN = x.URN
            })
            .ToList();
        }

        /// <summary>
        /// Returns short patient information by their <paramref name="patientURN"/>
        /// </summary>
        /// <param name="patientURN"></param>
        /// <returns></returns>
        public PublicPatient? GetPatient(int patientURN)
        {
            var patient = _patientRepository.GetPatient(patientURN);

            if (patient == null)
            {
                return null;
            }

            return new PublicPatient
            {
                Id = patient.Id,
                DateOfBirth = patient.DateOfBirthUTC,
                DisplayName = GetDisplayName(patient.DisplayFirstName, patient.DisplayLastName),
                FullName = patient.FullName,
                URN = patient.URN
            };
        }

        /// <summary>
        /// Returns the full details of a patient by their <paramref name="patientURN"/>, including the bed they may be currently in if admitted, and any comments added
        /// </summary>
        /// <param name="patientURN"></param>
        /// <returns></returns>
        public PublicPatientDetails? GetPatientDetails(int patientURN)
        {
            var patient = _patientRepository.GetPatientDetails(patientURN);

            if (patient == null)
            {
                return null;
            }

            return new PublicPatientDetails
            {
                Id = patient.Id,
                DateOfBirth = patient.DateOfBirthUTC,
                DisplayName = GetDisplayName(patient.DisplayFirstName, patient.DisplayLastName),
                FullName = patient.FullName,
                URN = patient.URN
            };
        }

        /// <summary>
        /// Adds a <paramref name="comment"/> to a patient by <paramref name="patientURN"/>, and associates it to an employee by <paramref name="employeeID"/>
        /// </summary>
        /// <param name="comment"></param>
        /// <param name="patientURN"></param>
        /// <param name="employeeID"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public object AddCommentToPatient(string comment, int patientURN, int employeeID)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns a patients display name given their first and last display names.
        /// <para>
        /// If <paramref name="displayLastName"/> is null, <paramref name="displayFirstName"/> will be returned,
        /// otherwise "<paramref name="displayFirstName"/> <paramref name="displayLastName"/>" will be returned
        /// </para>
        /// </summary>
        /// <param name="displayFirstName"></param>
        /// <param name="displayLastName"></param>
        /// <returns></returns>
        private string GetDisplayName(string displayFirstName, string? displayLastName = null)
        {
            if (string.IsNullOrWhiteSpace(displayLastName))
            {
                return displayFirstName;
            }

            return $"{displayFirstName} {displayLastName}";
        }
    }
}
