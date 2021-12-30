using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nulah.HospitalHelper.Data;
using Nulah.HospitalHelper.Lib;
using System;
using System.Collections.Generic;
using System.IO;

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
            var bedManager = TestHelpers.GetBedManager();
            var beds = bedManager.GetBeds();

            Assert.IsTrue(beds.Count > 0);
            Assert.IsTrue(beds.Count == 8);
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