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

        [TestMethod]
        public void LoginUser_1_WithCorrectPassword_ShouldReturn_True()
        {
            var userManager = TestHelpers.GetUserManager();
            var userLoginToken = userManager.Login(1, "Bas1c_P@ssw0rd");

            Assert.IsNotNull(userLoginToken);

            var userLoggedIn = userManager.CheckUserLogin(1, userLoginToken);

            Assert.IsTrue(userLoggedIn);
        }

        [TestMethod]
        public void LoginUser_1_Twice_ShouldReturn_Null_On2ndLogin()
        {
            var userManager = TestHelpers.GetUserManager();
            var userLoginToken = userManager.Login(1, "Bas1c_P@ssw0rd");

            Assert.IsNotNull(userLoginToken);

            userLoginToken = userManager.Login(1, "Bas1c_P@ssw0rd");

            Assert.IsNull(userLoginToken);
        }

        [TestMethod]
        public void LoginUser_1_WithBadPassword_ShouldReturn_Null()
        {
            var userManager = TestHelpers.GetUserManager();
            var userLoginToken = userManager.Login(1, "Wrong password");

            Assert.IsNull(userLoginToken);
        }

        [TestMethod]
        public void LoginUser_1_WithNullPassword_ShouldReturn_Null()
        {
            var userManager = TestHelpers.GetUserManager();
            var userLoginToken = userManager.Login(1, null);

            Assert.IsNull(userLoginToken);
        }

        [TestMethod]
        public void LoginUser_1_WithWhitespacePassword_ShouldReturn_Null()
        {
            var userManager = TestHelpers.GetUserManager();
            var userLoginToken = userManager.Login(1, "                      ");

            Assert.IsNull(userLoginToken);
        }

        [TestMethod]
        public void LogoutUser_1_NotLoggedIn_ShouldReturn_False()
        {
            var userManager = TestHelpers.GetUserManager();
            var userLoggedOut = userManager.Logout(1);

            Assert.IsFalse(userLoggedOut);
        }

        [TestMethod]
        public void LogoutUser_1_LoggedIn_ShouldReturn_True()
        {
            var userManager = TestHelpers.GetUserManager();
            var userLoginToken = userManager.Login(1, "Bas1c_P@ssw0rd");

            Assert.IsNotNull(userLoginToken);

            var userLoggedOut = userManager.Logout(1);

            Assert.IsTrue(userLoggedOut);
        }
    }
}
