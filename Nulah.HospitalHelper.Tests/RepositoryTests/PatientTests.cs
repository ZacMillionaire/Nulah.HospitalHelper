using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nulah.HospitalHelper.Core;
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


            Assert.AreEqual("John", patients!.DisplayFirstName);
            Assert.AreEqual("Doe", patients!.DisplayLastName);
            Assert.AreEqual(Formatters.PersonNameToDisplayFormat("John", "Doe"), patients!.DisplayName);
            Assert.AreEqual("John Doe", patients!.FullName);

            Assert.IsTrue(patients!.DisplayURN == 83524.ToString("D7"));
        }

        [TestMethod]
        public void GetFullPatientDetailsByURN_83524_ShouldReturn_PatientFullDetails()
        {
            var patientManager = TestHelpers.GetPatientManager();
            var patientFullDetails = patientManager.GetPatientDetails(83524);

            Assert.AreEqual("John", patientFullDetails!.DisplayFirstName);
            Assert.AreEqual("Doe", patientFullDetails!.DisplayLastName);
            Assert.AreEqual(Formatters.PersonNameToDisplayFormat("John", "Doe"), patientFullDetails!.DisplayName);
            Assert.AreEqual("John Doe", patientFullDetails!.FullName);

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


        [TestMethod]
        public void CreateNewPatient_ShouldReturn_NewPatient()
        {
            var patientManager = TestHelpers.GetPatientManager();

            // Don't reuse the for new patient to ensure 2 separate calls produce the same date time
            var DoB_Brisbane = TestHelpers.CreateDateTimeForTimezone(new DateTime(1989, 10, 2, 20, 0, 0), TimeZoneInfo.Utc);

            var newPatient = patientManager.CreateNewPatient("Pascal Nulah", "Pascal", "Nulah", TestHelpers.CreateDateTimeForTimezone(new DateTime(1989, 10, 2, 20, 0, 0), TimeZoneInfo.GetSystemTimeZones().First(x => x.StandardName == "E. Australia Standard Time")));

            // URN is high due to a higher number URN being created in test data.
            // This is not an arbitrary number
            Assert.AreEqual(83525, newPatient.URN);
            Assert.AreEqual("Pascal Nulah", newPatient.FullName);
            Assert.AreEqual("Pascal Nulah", newPatient.DisplayName);
            Assert.AreEqual("Pascal", newPatient.DisplayFirstName);
            Assert.AreEqual("Nulah", newPatient.DisplayLastName);

            // Check that the DoB added for a date of birth in Brisbane matches the local time conversion from the patient manager
            Assert.AreEqual(DoB_Brisbane.ToShortTimeString(), newPatient.DateOfBirth.ToLocalTime().ToShortTimeString());
            Assert.AreEqual(DoB_Brisbane.ToShortDateString(), newPatient.DateOfBirth.ToLocalTime().ToShortDateString());
        }


        [TestMethod]
        public void CreateNewPatient_WithNoLastName_ShouldReturn_NewPatient()
        {
            var patientManager = TestHelpers.GetPatientManager();

            // Don't reuse the for new patient to ensure 2 separate calls produce the same date time
            var DoB_Brisbane = TestHelpers.CreateDateTimeForTimezone(new DateTime(1989, 10, 2, 20, 0, 0), TimeZoneInfo.Utc);

            var newPatient = patientManager.CreateNewPatient("Pascal Nulah", "Pascal", null, TestHelpers.CreateDateTimeForTimezone(new DateTime(1989, 10, 2, 20, 0, 0), TimeZoneInfo.GetSystemTimeZones().First(x => x.StandardName == "E. Australia Standard Time")));

            // URN is high due to a higher number URN being created in test data.
            // This is not an arbitrary number
            Assert.AreEqual(83525, newPatient.URN);
            Assert.AreEqual("Pascal Nulah", newPatient.FullName);
            Assert.AreEqual("Pascal", newPatient.DisplayName);
            Assert.AreEqual("Pascal", newPatient.DisplayFirstName);
            Assert.IsNull(newPatient.DisplayLastName);

            // Check that the DoB added for a date of birth in Brisbane matches the local time conversion from the patient manager
            Assert.AreEqual(DoB_Brisbane.ToShortTimeString(), newPatient.DateOfBirth.ToLocalTime().ToShortTimeString());
            Assert.AreEqual(DoB_Brisbane.ToShortDateString(), newPatient.DateOfBirth.ToLocalTime().ToShortDateString());
        }

        [TestMethod]
        public void AddPatient_83525_ToBed_2_ShouldReturn_True()
        {
            var patientManager = TestHelpers.GetPatientManager();

            var newPatient = patientManager.CreateNewPatient("Pascal Nulah", "Pascal", "Nulah", TestHelpers.CreateDateTimeForTimezone(new DateTime(1989, 10, 2, 20, 0, 0), TimeZoneInfo.GetSystemTimeZones().First(x => x.StandardName == "E. Australia Standard Time")));

            var addToBedResult = patientManager.AddPatientToBed(newPatient.URN, 2);

            Assert.IsTrue(addToBedResult);
        }

        [TestMethod]
        public void AddPatient_83525_ToBed_1_ShouldReturn_False()
        {
            var patientManager = TestHelpers.GetPatientManager();

            var newPatient = patientManager.CreateNewPatient("Pascal Nulah", "Pascal", "Nulah", TestHelpers.CreateDateTimeForTimezone(new DateTime(1989, 10, 2, 20, 0, 0), TimeZoneInfo.GetSystemTimeZones().First(x => x.StandardName == "E. Australia Standard Time")));

            var addToBedResult = patientManager.AddPatientToBed(newPatient.URN, 1);

            Assert.IsFalse(addToBedResult);
        }

        [TestMethod]
        public void AddPatient_83525_ToBedThatDoesNotExist_100_ShouldReturn_False()
        {
            var patientManager = TestHelpers.GetPatientManager();

            var newPatient = patientManager.CreateNewPatient("Pascal Nulah", "Pascal", "Nulah", TestHelpers.CreateDateTimeForTimezone(new DateTime(1989, 10, 2, 20, 0, 0), TimeZoneInfo.GetSystemTimeZones().First(x => x.StandardName == "E. Australia Standard Time")));

            var addToBedResult = patientManager.AddPatientToBed(newPatient.URN, 100);

            Assert.IsFalse(addToBedResult);
        }

        [TestMethod]
        public void AddPatientAlreadyInABed_83524_ToBedThatIsFree_2_ShouldReturn_False()
        {
            var patientManager = TestHelpers.GetPatientManager();

            var addToBedResult = patientManager.AddPatientToBed(83524, 2);

            Assert.IsFalse(addToBedResult);
        }

        [TestMethod]
        public void PatientThatDoesNotExist_9999_ToBed_2_ShouldReturn_False()
        {
            var patientManager = TestHelpers.GetPatientManager();

            var addToBedResult = patientManager.AddPatientToBed(9999, 2);

            Assert.IsFalse(addToBedResult);
        }

        [TestMethod]
        public void RemovePatient_83524_FromBed_1_ShouldReturn_True()
        {
            var patientManager = TestHelpers.GetPatientManager();

            var removeFromBedResult = patientManager.RemovePatientFromBed(83524, 1);

            Assert.IsTrue(removeFromBedResult);
        }

        [TestMethod]
        public void RemovePatient_83524_FromBed_2_ShouldReturn_False()
        {
            var patientManager = TestHelpers.GetPatientManager();

            var removeFromBedResult = patientManager.RemovePatientFromBed(83524, 2);

            Assert.IsFalse(removeFromBedResult);
        }

        [TestMethod]
        public void RemovePatient_83524_ToBedThatDoesNotExist_100_ShouldReturn_False()
        {
            var patientManager = TestHelpers.GetPatientManager();

            var removeFromBedResult = patientManager.RemovePatientFromBed(83524, 100);

            Assert.IsFalse(removeFromBedResult);
        }

        [TestMethod]
        public void RemovePatientNotAssignedToBed_3000_2_ShouldReturn_False()
        {
            var patientManager = TestHelpers.GetPatientManager();

            var addToBedResult = patientManager.AddPatientToBed(30000, 2);

            Assert.IsFalse(addToBedResult);
        }
    }
}
