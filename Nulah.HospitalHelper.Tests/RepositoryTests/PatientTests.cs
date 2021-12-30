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
            Assert.IsTrue(patients!.DisplayURN == 83524.ToString("D7"));
        }

        [TestMethod]
        public void GetFullPatientDetailsByURN_83524_ShouldReturn_PatientFullDetails()
        {
            var patientManager = TestHelpers.GetPatientManager();
            var patientFullDetails = patientManager.GetPatientDetails(83524);

            Assert.IsTrue(patientFullDetails!.Comments.Count == 4);
            Assert.IsTrue(patientFullDetails.Comments[0].Comment == "Admitted");
            Assert.IsTrue(patientFullDetails.Comments[1].Comment == "Temp checked");
            Assert.IsTrue(patientFullDetails.Comments[2].Comment == "Blood pressure checked");
            Assert.IsTrue(patientFullDetails.Comments[3].Comment == "Discharged");

            Assert.IsTrue(patientFullDetails.PresentingIssue == "Nausea, dizziness");

            Assert.IsTrue(patientFullDetails.DateOfBirth == TestHelpers.CreateDateTimeForTimezone(new DateTime(1980, 1, 1), TimeZoneInfo.Utc));

            Assert.IsTrue(patientFullDetails.BedNumber == 1);
        }

        [TestMethod]
        public void AddCommentToPatientURN_83524_ShouldReturn_PatientFullDetails_With1Comment()
        {
            var patientManager = TestHelpers.GetPatientManager();
            var addedComment = patientManager.AddCommentToPatient("Admitted", 83524, 1);

            Assert.IsTrue(addedComment!.Comment == "Admitted");
            Assert.IsTrue(addedComment.Id == 7);
        }

        [TestMethod]
        public void AddCommentsToPatientURN_83524_ShouldReturn_PatientFullDetails_WithMutlipleComments()
        {
            var patientManager = TestHelpers.GetPatientManager();
            patientManager.AddCommentToPatient("Admitted", 83524, 1);
            patientManager.AddCommentToPatient("Sent for X-Ray", 83524, 1);
            patientManager.AddCommentToPatient("Waiting for X-Ray results", 83524, 1);


            var patientFullDetails = patientManager.GetPatientDetails(83524);

            Assert.IsTrue(patientFullDetails!.Comments.Count == 7);
            Assert.IsTrue(patientFullDetails.Comments[4].Comment == "Admitted");
            Assert.IsTrue(patientFullDetails.Comments[5].Comment == "Sent for X-Ray");
            Assert.IsTrue(patientFullDetails.Comments[6].Comment == "Waiting for X-Ray results");
        }
    }
}
