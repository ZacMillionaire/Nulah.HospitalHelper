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
        public void GetPatientByURN_ShouldReturn_PatientURN()
        {
            var patientManager = TestHelpers.GetPatientManager();
            var patients = patientManager.GetPatient(1);

            Assert.IsTrue(patients != null);
            Assert.IsTrue(patients!.DisplayURN == "1".PadLeft(7, '0'));
        }
    }
}
