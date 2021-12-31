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

        public List<PublicBed> GetBeds()
        {
            var beds = _bedRepository.GetBeds();

            return beds.Select(x => new PublicBed
            {
                BedNumber = x.Number,
                BedStatus = x.BedStatus,
                Patient = CreatePublicPatientDetails(x.PatientURN)
            })
            .ToList();
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

        public PublicBed? GetBedById(int bedId)
        {
            var bed = _bedRepository.GetBedByNumber(bedId);

            if (bed == null)
            {
                return null;
            }

            return new PublicBed
            {
                Id = bed.Id,
                BedNumber = bed.Number,
                BedStatus = bed.BedStatus
            };
        }
    }
}
