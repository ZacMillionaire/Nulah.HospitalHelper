using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nulah.HospitalHelper.Api.Controllers;
using Nulah.HospitalHelper.Frontend.Controllers.Home;
using Nulah.HospitalHelper.Frontend.Models;
using Nulah.HospitalHelper.Frontend.Models.User;
using System.Security.Claims;

namespace Nulah.HospitalHelper.Frontend.Controllers.User
{
    public class UserController : Controller
    {
        private readonly UserApiController _userApi;

        public UserController(UserApiController userApiController)
        {
            _userApi = userApiController;
        }

        [HttpGet]
        [Route("Login")]
        [AllowAnonymous]
        public IActionResult Login([FromQuery] string? returnUrl = null)
        {
            return View(new LoginFormData
            {
                EmployeeId = null,
                ReturnUrl = returnUrl
            });
        }

        [HttpPost]
        [Route("Login")]
        [AllowAnonymous]
        public async Task<IActionResult> LoginUser([FromForm] LoginFormData loginFormData)
        {
            if (loginFormData.EmployeeId == null)
            {
                ViewBag.Error = "Invalid employee ID/password";
                return RedirectToAction(nameof(UserController.Login));
            }

            var loginToken = _userApi.LoginUser(loginFormData.EmployeeId.Value, loginFormData.Password);

            if (loginToken == null)
            {
                ViewBag.Error = "Invalid employee ID/password";
                return RedirectToAction(nameof(UserController.Login));
            }

            var claims = new List<Claim> {
                new Claim("EmployeeId", loginFormData.EmployeeId.Value.ToString()),
                new Claim("LoginToken", loginToken)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties()
            {
                IsPersistent = true
            });

            return LocalRedirect(loginFormData.ReturnUrl ?? "/");
        }

        [HttpGet]
        [Route("Logout")]
        [LazyUserAuthorise]
        public async Task<IActionResult> Logout()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                var employeeId = int.Parse(User.Claims.First(x => x.Type == "EmployeeId").Value);
                var loginToken = User.Claims.First(x => x.Type == "LoginToken").Value;

                var userLoggedOut = _userApi.LogoutUser(employeeId, loginToken);

                // Shouldn't do this, a logout should always destroy any login artefacts,
                // but this authentication system is purposefully bad
                if (userLoggedOut == true)
                {
                    await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                    return RedirectToAction(nameof(UserController.Login));
                }
            }
            return RedirectToAction("/");
        }
    }
}
