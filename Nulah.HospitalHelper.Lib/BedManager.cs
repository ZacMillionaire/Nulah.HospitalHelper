using Nulah.HospitalHelper.Core;
using Nulah.HospitalHelper.Core.Interfaces;
using Nulah.HospitalHelper.Core.Models;
using Nulah.HospitalHelper.Core.Models.Data;
using Nulah.HospitalHelper.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nulah.HospitalHelper.Lib
{
    public class BedManager
    {
        private readonly IBedRepository _bedRepository;
        private readonly IPatientRepository _patientRepository;

        public BedManager(IBedRepository bedRepository, IPatientRepository patientRepository)
        {
            _bedRepository = bedRepository;
            _patientRepository = patientRepository;
        }

        /// <summary>
        /// Returns all beds available, including any admitted patients in <see cref="BedStatus.InUse"/> beds
        /// </summary>
        /// <returns></returns>
        public List<PublicBed> GetBeds()
        {
            var beds = _bedRepository.GetBeds();

            return beds.Select(x => new PublicBed
            {
                Id = x.Id,
                BedNumber = x.Number,
                BedStatus = x.BedStatus,
                Patient = CreatePublicPatientDetails(x.PatientURN)
            })
            .ToList();
        }

        /// <summary>
        /// Returns details for the given bed, if the bed has a status of <see cref="BedStatus.InUse"/>, patient details will also be included
        /// </summary>
        /// <returns></returns>
        public PublicBed? GetBedById(int bedNumber)
        {
            var bed = _bedRepository.GetBedByNumber(bedNumber);

            if (bed == null)
            {
                return null;
            }

            return new PublicBed
            {
                Id = bed.Id,
                BedNumber = bed.Number,
                BedStatus = bed.BedStatus,
                Patient = CreatePublicPatientDetails(bed.PatientURN)
            };
        }

        private PublicPatientDetails? CreatePublicPatientDetails(int? patientURN)
        {
            if (patientURN == null)
            {
                return null;
            }

            var patientDetails = _patientRepository.GetPatientDetails((int)patientURN);

            if (patientDetails == null)
            {
                return null;
            }

            return new PublicPatientDetails
            {
                Id = patientDetails.Id,
                DateOfBirth = patientDetails.DateOfBirthUTC,
                DisplayFirstName = patientDetails.DisplayFirstName,
                DisplayLastName = patientDetails.DisplayLastName,
                DisplayName = Formatters.PersonNameToDisplayFormat(patientDetails.DisplayFirstName, patientDetails.DisplayLastName),
                FullName = patientDetails.FullName,
                URN = patientDetails.URN,
                Comments = patientDetails.Comments
                    .Select(x => new PublicPatientComment
                    {
                        Comment = x.Comment,
                        DateTimeUTC = x.DateTimeUTC,
                        Id = x.Id,
                        Nurse = x.Nurse
                    })
                    .ToList(),
                PresentingIssue = patientDetails.HealthDetails?.PresentingIssue
            };
        }
    }
}
