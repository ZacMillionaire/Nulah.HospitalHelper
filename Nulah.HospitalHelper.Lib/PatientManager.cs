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
                DateOfBirth = x.DateOfBirth,
                DisplayName = $"{x.DisplayFirstName} {x.DisplayLastName}",
                URN = x.URN
            })
            .ToList();
        }

        /// <summary>
        /// Returns short patient information by their URN
        /// </summary>
        /// <param name="URN"></param>
        /// <returns></returns>
        public PublicPatient? GetPatient(int URN)
        {
            var patient = _patientRepository.GetPatient(URN);

            if (patient == null)
            {
                return null;
            }

            return new PublicPatient
            {
                Id = patient.Id,
                DateOfBirth = patient.DateOfBirth,
                DisplayName = GetDisplayName(patient.DisplayFirstName, patient.DisplayLastName),
                FullName = patient.FullName,
                URN = patient.URN
            };
        }

        /// <summary>
        /// Returns the full details of a patient, including the bed they may be currently in if admitted, and any comments added
        /// </summary>
        /// <param name="URN"></param>
        /// <returns></returns>
        public PublicPatientDetails? GetPatientDetails(int URN)
        {
            var patient = _patientRepository.GetPatientDetails(URN);

            if (patient == null)
            {
                return null;
            }

            return new PublicPatientDetails
            {
                Id = patient.Id,
                DateOfBirth = patient.DateOfBirth,
                DisplayName = GetDisplayName(patient.DisplayFirstName, patient.DisplayLastName),
                FullName = patient.FullName,
                URN = patient.URN
            };
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
