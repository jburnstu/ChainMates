using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using ChainMates.Server;
using ChainMates.Server.DTOs;
using System.Diagnostics;
using System.Security.Claims;

namespace ChainMates.Server.Services
    ////DISCLOSURE: used AI to generate this section, as ever ensuring that i understood how it worked after the fact.
{
    public class CurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public int? UserId
        {
            get
            {
                var user = _httpContextAccessor.HttpContext?.User;

                var claim = user?.FindFirst(ClaimTypes.NameIdentifier);

                return claim != null ? int.Parse(claim.Value) : (int?)null;
            }
        }

    }
}
