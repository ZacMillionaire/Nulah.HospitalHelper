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
        private static BedManager _bedManager;


        // Singleton for tests
        static BedManagerTests()
        {
            var testDatabaseLocation = "./hospitalHelperTest.db";

            if (File.Exists(testDatabaseLocation))
            {
                File.Delete(testDatabaseLocation);
            }

            var dataRepository = new SqliteDataRepository($"Data Source={testDatabaseLocation};");
            dataRepository.SeedDatabase();
            var bedRepository = new BedRepository(dataRepository);

            _bedManager = new BedManager(bedRepository);
        }

        [TestMethod]
        public void GetAllBeds_ShouldReturn_AllBeds()
        {
            var beds = _bedManager.GetBeds();

            Assert.IsTrue(beds.Count > 0);
            Assert.IsTrue(beds.Count == 3);
        }

        [TestMethod]
        public void GetBedByNumber_1_ShouldReturn_Bed1()
        {
            var beds = _bedManager.GetBedById(1);

            Assert.IsTrue(beds != null);
            Assert.IsTrue(beds.BedNumber == 1);
            Assert.IsTrue(beds.Id == new Guid(1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1));
        }

        [TestMethod]
        public void GetBedByInvalidNumber_10_ShouldReturn_null()
        {
            var beds = _bedManager.GetBedById(10);

            Assert.IsTrue(beds == null);
        }
    }
}