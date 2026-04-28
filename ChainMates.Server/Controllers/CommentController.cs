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

        private readonly CommentService _commentService;
        private readonly CurrentUserService _currentUserService;

        public CommentController( CurrentUserService currentUserService, CommentService commentService)
        {
            _commentService = commentService;
            _currentUserService = currentUserService;
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
            var data = await _commentService.CreateAndSubmitComment(dto, authorId);
            return Ok(data);

        }
    }
}
