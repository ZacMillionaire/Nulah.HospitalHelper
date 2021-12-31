using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nulah.HospitalHelper.Data;
using Nulah.HospitalHelper.Lib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Nulah.HospitalHelper.Tests.RepositoryTests
{
    [TestClass]
    public class BedManagerTests
    {
        [TestInitialize]
        public void InitializeTests()
        {
            TestHelpers.ReseedDatabase();
        }

        [TestMethod]
        public void GetAllBeds_ShouldReturn_AllBeds()
        {
            var tz = TimeZoneInfo.GetSystemTimeZones().First(x => x.StandardName == "E. Australia Standard Time");

            var bedManager = TestHelpers.GetBedManager();
            var beds = bedManager.GetBeds();

            Assert.IsTrue(beds.Count > 0);
            Assert.IsTrue(beds.Count == 8);

            var bedForJohnDoe = beds[0];
            var bedForLornaSmith = beds[4];
            var bedForDianaMay = beds[5];

            Assert.AreEqual("John Doe", bedForJohnDoe.Patient!.DisplayName);
            Assert.AreEqual("Nausea, dizziness", bedForJohnDoe.Patient!.PresentingIssue);
            Assert.AreEqual("Blood pressure checked", bedForJohnDoe.LastComment);
            Assert.AreEqual(TestHelpers.CreateDateTimeForTimezone(new DateTime(2020, 2, 2, 10, 25, 0), tz).ToUniversalTime(), bedForJohnDoe.LastUpdatedUTC);

            Assert.AreEqual("Lorna Smith", bedForLornaSmith.Patient!.DisplayName);
            Assert.AreEqual("Broken Leg", bedForLornaSmith.Patient!.PresentingIssue);
            Assert.AreEqual("X-Ray waiting results", bedForLornaSmith.LastComment);
            Assert.AreEqual(TestHelpers.CreateDateTimeForTimezone(new DateTime(2020, 2, 2, 7, 30, 25), tz).ToUniversalTime(), bedForLornaSmith.LastUpdatedUTC);

            Assert.AreEqual("Diana May", bedForDianaMay.Patient!.DisplayName);
            Assert.AreEqual("High fever", bedForLornaSmith.Patient!.PresentingIssue);
            Assert.AreEqual("Medication supplied", bedForLornaSmith.LastComment);
            Assert.AreEqual(TestHelpers.CreateDateTimeForTimezone(new DateTime(2020, 2, 2, 9, 45, 25), tz).ToUniversalTime(), bedForDianaMay.LastUpdatedUTC);
        }

        [TestMethod]
        public void GetBedByNumber_1_ShouldReturn_Bed1()
        {
            var bedManager = TestHelpers.GetBedManager();
            var beds = bedManager.GetBedById(1);

            Assert.IsTrue(beds != null);
            Assert.IsTrue(beds.BedNumber == 1);
        }

        [TestMethod]
        public void GetBedByInvalidNumber_10_ShouldReturn_null()
        {
            var bedManager = TestHelpers.GetBedManager();
            var bed = bedManager.GetBedById(10);

            Assert.IsTrue(bed == null);
        }
    }
}