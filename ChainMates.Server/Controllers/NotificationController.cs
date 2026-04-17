using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ChainMates.Server.DTOs.Notification;
using ChainMates.Server.Services;
using System.Diagnostics;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ChainMates.Server.Controllers
{
    [Route("chainmates/notifications")]
    [ApiController]
    public class NotificationController : ControllerBase
    {

        private readonly AppDbContext _context;
        private readonly NotificationService _service;
        private readonly CurrentUserService _currentUserService;

        public NotificationController(AppDbContext context, CurrentUserService currentUserService)
        {
            Debug.WriteLine("in service constructor");
            _context = context;
            _service = new NotificationService(context);
            _currentUserService = currentUserService;
        }




        [Authorize]
        [HttpGet]

        public async Task<IActionResult> GetNotifications()
        {
            int authorId = _currentUserService.UserId ?? 0;

            if (authorId == 0)
            {
                return Unauthorized();
            }
            var data = await _service.GetRecentNotificationsByRecipient(authorId);
            return Ok(data);

        }


    }
}
