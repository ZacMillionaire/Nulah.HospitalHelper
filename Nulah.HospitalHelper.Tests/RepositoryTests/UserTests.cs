using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nulah.HospitalHelper.Tests.RepositoryTests
{
    [TestClass]
    public class UserTests
    {
        [TestInitialize]
        public void InitializeTests()
        {
            TestHelpers.ReseedDatabase();
        }

        [TestMethod]
        public void CreateNewUser_ForEmployeeId_1_ShouldReturn_False()
        {
            var userManager = TestHelpers.GetUserManager();
            var createdUser = userManager.CreateUser(1, "Bas1c_P@ssw0rd");

            Assert.IsFalse(createdUser);
        }

        [TestMethod]
        public void CreateNewUser_ForEmployeeId_ThatDoesntExist_ShouldReturn_False()
        {
            var userManager = TestHelpers.GetUserManager();
            var createdUser = userManager.CreateUser(1000, "Bas1c_P@ssw0rd");

            Assert.IsFalse(createdUser);
        }

        [TestMethod]
        public void CreateNewUser_ForNewEmployee_ShouldReturn_True()
        {
            var userManager = TestHelpers.GetUserManager();
            var employeeManager = TestHelpers.GetEmployeeManager();

            var createdEmployee = employeeManager.CreateEmployee("Pascal Nulah", "Pascal", "Nulah");

            var createdUser = userManager.CreateUser(createdEmployee!.EmployeeId, "Bas1c_P@ssw0rd");

            Assert.IsTrue(createdUser);
        }
    }
}
