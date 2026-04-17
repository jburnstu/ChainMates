using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ChainMates.Server.DTOs.Comment;
using ChainMates.Server.Services;
using System.Diagnostics;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ChainMates.Server.Controllers
{
    [Route("chainmates/comments")]
    [ApiController]
    public class CommentController : ControllerBase
    {

        private readonly AppDbContext _context;
        private readonly CommentService _service;
        private readonly CurrentUserService _currentUserService;
        private readonly NotificationService _notificationService;

        public CommentController(AppDbContext context, CurrentUserService currentUserService, NotificationService notificationService)
        {
            Debug.WriteLine("in service constructor");
            _context = context;
            _service = new CommentService(context);
            _currentUserService = currentUserService;
            _notificationService = notificationService;
        }



        // POST api/<StoryController>
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CommentCreationAndSubmissionDto dto)
        {
            int authorId = _currentUserService.UserId ?? 0;

            if (authorId == 0)
            {
                return Unauthorized();
            }
            Debug.WriteLine("InCommentController");
            Debug.WriteLine(dto.ParentId);
            var data = await _service.CreateAndSubmitComment(dto, authorId);
            await _notificationService.NotifyCommentPosted(dto.CommentTypeId, dto.ParentId, authorId);

            return Ok(data);

        }
    }
}
