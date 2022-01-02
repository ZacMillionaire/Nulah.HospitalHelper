using Microsoft.AspNetCore.Authorization;
using Nulah.HospitalHelper.Api.Controllers;

namespace Nulah.HospitalHelper.Api.Models
{
    public class LazyApiAuthoriseAttribute : AuthorizeAttribute
    {
        public LazyApiAuthoriseAttribute()
        {
            AuthenticationSchemes = LazyApiAuthentication.AuthenticationSchemes;
        }
    }
}
