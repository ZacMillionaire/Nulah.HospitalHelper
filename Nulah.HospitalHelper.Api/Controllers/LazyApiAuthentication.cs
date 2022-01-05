using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Nulah.HospitalHelper.Api.Models;
using Nulah.HospitalHelper.Lib;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace Nulah.HospitalHelper.Api.Controllers
{
    public class LazyApiAuthentication : AuthenticationHandler<LazyApiSchemeOptions>
    {
        public static string AuthenticationSchemes = "LaziApi";

        private readonly UserManager _userManager;

        public LazyApiAuthentication(IOptionsMonitor<LazyApiSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock, UserManager userManger)
            : base(options, logger, encoder, clock)
        {
            _userManager = userManger;
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (Request.Headers.ContainsKey("Authorization") == false)
            {
                return Task.FromResult(AuthenticateResult.Fail("No Authorization header supplied"));
            }

            var authHeader = Request.Headers["Authorization"].ToString();

            if (authHeader.Contains("Bearer ") == false)
            {
                return Task.FromResult(AuthenticateResult.Fail("Invalid Authorization header"));
            }

            // Terribly split on the 'Bearer ' portion of the header
            var apiToken = authHeader.Split("Bearer ");

            // Lazily check that the 2nd part of the split matches our api token
            if (apiToken[1] == "API-TOKEN-SUPER-SECURE")
            {
                // Empty claims identity
                var claimsIdentity = new ClaimsIdentity(new Claim[0] { }, nameof(LazyApiAuthentication));
                return Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(new ClaimsPrincipal(claimsIdentity), Scheme.Name)));
            }


            return Task.FromResult(AuthenticateResult.Fail("Unauthorized access prohibited"));
        }
    }
}

