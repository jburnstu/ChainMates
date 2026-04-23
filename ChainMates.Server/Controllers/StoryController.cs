using Microsoft.AspNetCore.Mvc;
using ChainMates.Server.DTOs.Story;
using ChainMates.Server.Services;
using System.Diagnostics;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ChainMates.Server.Controllers
{
    [Route("chainmates/stories")]
    [ApiController]
    public class StoryController : ControllerBase
    {

        private readonly AppDbContext _context;
        private readonly StoryService _service;
        private readonly CurrentUserService _currentUserService;

        public StoryController(AppDbContext context, CurrentUserService currentUserService)
        {
            Debug.WriteLine("in service constructor");
            _context = context;
            _service = new StoryService(context);
            _currentUserService = currentUserService;
        }


        // GET: api/stories
        [HttpGet]
        public async Task<IActionResult> GetStories()
        {
            var data = await _service.GetStories();
            return Ok(data);
        }

        //// GET api/stories/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetStoryById(int id)
        {
            var data = await _service.GetStoryById(id);

            return Ok(data);
        }

        [HttpGet("{storyId}/structure")]
        public async Task<IActionResult> GetStoryStructure(int storyId)
        {
            var data = await _service.GetStoryStructure(storyId);
            return Ok(data);

        }

        // POST api/<StoryController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] StoryDto dto)
        {
            int authorId = _currentUserService.UserId ?? 0;

            if (authorId == 0)
            {
                return Unauthorized();
            }
            var data = await _service.CreateStoryWithInitialSegment(dto ,authorId);
            return Ok(data);

        }

    }
}
