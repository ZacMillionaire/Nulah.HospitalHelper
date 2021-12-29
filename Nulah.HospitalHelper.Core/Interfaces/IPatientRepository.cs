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
        public List<PublicPatient> GetPatients();
        public PublicPatient GetPatient(Guid patientId);
        public PublicPatientDetails GetPatientDetails(Guid patientId);
        public Guid AddCommentToPatient(Guid patientId);
        public bool RemoveCommentFromPatient(Guid commentId, Guid patientId);
    }
}
