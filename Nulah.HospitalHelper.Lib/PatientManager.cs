using Nulah.HospitalHelper.Core;
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
        private readonly IBedRepository _bedRepository;

        public PatientManager(IPatientRepository patientRepository, IBedRepository bedRepository)
        {
            _patientRepository = patientRepository;
            _bedRepository = bedRepository;
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
                DisplayName = Formatters.PersonNameToDisplayFormat(patient.DisplayFirstName, patient.DisplayLastName),
                DisplayFirstName = patient.DisplayFirstName,
                DisplayLastName = patient.DisplayLastName,
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

            var bed = _bedRepository.GetBedForPatient(patientURN);

            return new PublicPatientDetails
            {
                Id = patient.Id,
                DateOfBirth = patient.DateOfBirthUTC,
                DisplayFirstName = patient.DisplayFirstName,
                DisplayLastName= patient.DisplayLastName,
                DisplayName = Formatters.PersonNameToDisplayFormat(patient.DisplayFirstName, patient.DisplayLastName),
                FullName = patient.FullName,
                URN = patient.URN,
                Comments = patient.Comments
                    .Select(x => new PublicPatientComment
                    {
                        Comment = x.Comment,
                        DateTimeUTC = x.DateTimeUTC,
                        Id = x.Id,
                        Nurse = x.Nurse
                    })
                    .ToList(),
                PresentingIssue = patient.HealthDetails?.PresentingIssue,
                BedId = bed?.Id,
                BedNumber = bed?.Number
            };
        }

        /// <summary>
        /// Creates a new patient
        /// </summary>
        /// <param name="fullName"></param>
        /// <param name="displayFirstName"></param>
        /// <param name="displayLastName"></param>
        /// <param name="dateOfBirthUTC"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public PublicPatient CreateNewPatient(string fullName, string displayFirstName, string? displayLastName, DateTime dateOfBirthUTC)
        {
            var newPatient = _patientRepository.CreatePatient(fullName, displayFirstName, displayLastName, dateOfBirthUTC);

            if (newPatient == null)
            {
                // Generic throw if we had a "silent error" that resulted in no patient being created.
                throw new Exception("Failed to create new patient");
            }

            return new PublicPatient
            {
                Id = newPatient.Id,
                DisplayName = Formatters.PersonNameToDisplayFormat(newPatient.DisplayFirstName, newPatient.DisplayLastName),
                FullName = newPatient.FullName,
                DateOfBirth = newPatient.DateOfBirthUTC,
                DisplayFirstName = newPatient.DisplayFirstName,
                DisplayLastName = newPatient.DisplayLastName,
                URN = newPatient.URN
            };
        }

        /// <summary>
        /// Adds a <paramref name="comment"/> to a patient by <paramref name="patientURN"/>, and associates it to an employee by <paramref name="employeeID"/>
        /// <para>
        /// This method assumes <paramref name="employeeID"/> has already been validated to exist.
        /// </para>
        /// </summary>
        /// <param name="comment"></param>
        /// <param name="patientURN"></param>
        /// <param name="employeeID"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public PatientComment? AddCommentToPatient(string comment, int patientURN, int employeeID)
        {
            // We assume the employeeId has already been validated via business logic higher up the call chain

            var createdComment = _patientRepository.AddCommentToPatient(comment, patientURN, employeeID);

            return createdComment;
        }

        /// <summary>
        /// Adds a patient to a bed, return true on success, or false if the bed is in use or does not exist
        /// </summary>
        /// <param name="patientURN"></param>
        /// <param name="bedNumber"></param>
        /// <returns></returns>
        public bool AddPatientToBed(int patientURN, int bedNumber)
        {
            var patient = _patientRepository.GetPatient(patientURN);

            if (patient == null)
            {
                return false;
            }

            return _bedRepository.AddPatientToBed(patientURN, bedNumber);
        }

        /// <summary>
        /// Removes a patient from a bed, returning true on success, or false if the patient is not assigned to that bed, or the bed does not exist
        /// </summary>
        /// <param name="patientURN"></param>
        /// <param name="bedNumber"></param>
        /// <returns></returns>
        public bool RemovePatientFromBed(int patientURN, int bedNumber)
        {
            var patient = _patientRepository.GetPatient(patientURN);

            if (patient == null)
            {
                return false;
            }

            return _bedRepository.RemovePatientFromBed(patientURN, bedNumber);
        }
    }
}
