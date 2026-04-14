using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using ChainMates.Server;
using ChainMates.Server.DTOs;
using System.Diagnostics;
using System.Security.Claims;

namespace ChainMates.Server.Services
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
                Console.WriteLine("Inside UserIdGet");
                Debug.WriteLine("DEBUG: Inside UserIdGet");
                var user = _httpContextAccessor.HttpContext?.User;

                var claim = user?.FindFirst(ClaimTypes.NameIdentifier);

                return claim != null ? int.Parse(claim.Value) : (int?)null;
            }
        }

    }
}
