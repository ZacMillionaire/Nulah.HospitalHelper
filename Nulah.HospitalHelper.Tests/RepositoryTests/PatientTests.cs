using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nulah.HospitalHelper.Core.Models;
using Nulah.HospitalHelper.Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nulah.HospitalHelper.Tests.RepositoryTests
{
    [TestClass]
    public class PatientTests
    {
        [TestInitialize]
        public void InitializeTests()
        {
            TestHelpers.ReseedDatabase();
        }

        [TestMethod]
        public void GetAllPatients_ShouldReturn_AllPatients()
        {
            var patientManager = TestHelpers.GetPatientManager();
            var patients = patientManager.GetPatients();

            Assert.IsTrue(patients.Count > 0);
            Assert.IsTrue(patients.Count == 3);
        }

        [TestMethod]
        public void GetPatientByURN_83524_ShouldReturn_PatientDetails()
        {
            var patientManager = TestHelpers.GetPatientManager();
            var patients = patientManager.GetPatient(83524);

            Assert.IsTrue(patients != null);
            Assert.IsTrue(patients!.DisplayURN == "83524".PadLeft(7, '0'));
        }

        [TestMethod]
        public void GetFullPatientDetailsByURN_83524_ShouldReturn_PatientFullDetails()
        {
            var patientManager = TestHelpers.GetPatientManager();
            var patients = patientManager.GetPatientDetails(83524);

            Assert.IsTrue(patients != null);
            Assert.IsTrue(patients!.DisplayURN == "83524".PadLeft(7, '0'));
        }

        [TestMethod]
        public void AddCommentToPatientURN_83524_ShouldReturn_PatientFullDetails_With1Comment()
        {
            var patientManager = TestHelpers.GetPatientManager();
            var patients = patientManager.AddCommentToPatient("Admitted", 83524, 1);

            Assert.IsTrue(false);
        }

        [TestMethod]
        public void AddCommentsToPatientURN_83524_ShouldReturn_PatientFullDetails_WithMutlipleComments()
        {
            var patientManager = TestHelpers.GetPatientManager();
            var patients = patientManager.GetPatientDetails(83524);

            Assert.IsTrue(false);
        }
    }
}
