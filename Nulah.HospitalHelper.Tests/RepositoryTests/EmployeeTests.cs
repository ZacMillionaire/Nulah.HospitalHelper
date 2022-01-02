﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nulah.HospitalHelper.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nulah.HospitalHelper.Tests.RepositoryTests
{
    [TestClass]
    public class EmployeeTests
    {
        [TestInitialize]
        public void InitializeTests()
        {
            TestHelpers.ReseedDatabase();
        }

        [TestMethod]
        public void CreateEmployee_PascalNulah_ShouldReturn_EmployeeForPascalNulah()
        {
            var employeeManager = TestHelpers.GetEmployeeManager();
            var createdEmployee = employeeManager.CreateEmployee("Pascal Nulah", "Pascal", "Nulah");

            Assert.IsNotNull(createdEmployee);
            Assert.IsNotNull(createdEmployee.DisplayLastName);

            Assert.AreEqual("Pascal Nulah", createdEmployee.FullName);
            Assert.AreEqual("Pascal", createdEmployee.DisplayFirstName);
            Assert.AreEqual("Nulah", createdEmployee.DisplayLastName);
            Assert.AreEqual("Pascal Nulah", createdEmployee.DisplayName);
        }

        [TestMethod]
        public void CreateEmployee_PascalNulah_PartialDisplayLastName_ShouldReturn_EmployeeForPascalNulah()
        {
            var employeeManager = TestHelpers.GetEmployeeManager();
            var createdEmployee = employeeManager.CreateEmployee("Pascal Nulah", "Pascal", "N.");

            Assert.IsNotNull(createdEmployee);
            Assert.IsNotNull(createdEmployee.DisplayLastName);

            Assert.AreEqual("Pascal Nulah", createdEmployee.FullName);
            Assert.AreEqual("Pascal", createdEmployee.DisplayFirstName);
            Assert.AreEqual("N.", createdEmployee.DisplayLastName);
            Assert.AreEqual("Pascal N.", createdEmployee.DisplayName);
        }

        [TestMethod]
        public void CreateEmployee_PascalNulah_PartialNoLastName_ShouldReturn_EmployeeForPascalNulah()
        {
            var employeeManager = TestHelpers.GetEmployeeManager();
            var createdEmployee = employeeManager.CreateEmployee("Pascal Nulah", "Pascal");

            Assert.IsNotNull(createdEmployee);
            Assert.IsNull(createdEmployee.DisplayLastName);

            Assert.AreEqual("Pascal Nulah", createdEmployee.FullName);
            Assert.AreEqual("Pascal", createdEmployee.DisplayFirstName);

            Assert.AreEqual("Pascal", createdEmployee.DisplayName);
        }
    }
}