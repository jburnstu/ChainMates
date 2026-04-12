using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using ReactApp1.Server;
using ReactApp1.Server.DTOs;
using System.Diagnostics;
using System.Security.Claims;

namespace ReactApp1.Server.Services
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
