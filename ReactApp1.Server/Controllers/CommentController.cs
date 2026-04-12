using Microsoft.AspNetCore.Mvc;
using ReactApp1.Server.DTOs;
using ReactApp1.Server.Services;
using System.Diagnostics;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ReactApp1.Server.Controllers
{
    [Route("chainmates/comments")]
    [ApiController]
    public class CommentController : ControllerBase
    {

        private readonly AppDbContext _context;
        private readonly CommentService _service;
        private readonly CurrentUserService _currentUserService;

        public CommentController(AppDbContext context, CurrentUserService currentUserService)
        {
            Debug.WriteLine("in service constructor");
            _context = context;
            _service = new CommentService(context);
            _currentUserService = currentUserService;
        }

        // POST api/<StoryController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CommentCreationDto dto)
        {
            int authorId = _currentUserService.UserId ?? 0;

            if (authorId == 0)
            {
                return Unauthorized();
            }

            var data = await _service.CreateComment(dto, authorId);
            return Ok(data);

        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> Patch([FromBody] CommentPatchDto dto, int id)
        {

            var comment = await _service.GetCommentById(id);
            var data = await _service.UpdateComment(comment, dto);
            return Ok(data);

        }
    }
}
