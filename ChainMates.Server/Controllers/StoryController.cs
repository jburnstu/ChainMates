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

        private readonly StoryService _storyService;
        private readonly CurrentUserService _currentUserService;
        private readonly StoryService _storyService;

        public StoryController( CurrentUserService currentUserService, StoryService storyService)
        {
            _currentUserService = currentUserService;
            _storyService = storyService;
        }


        // GET: api/stories
        [HttpGet]
        public async Task<IActionResult> GetStories()
        {
            var data = await _storyService.GetStories();
            return Ok(data);
        }

        //// GET api/stories/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetStoryById(int id)
        {
            var data = await _storyService.GetStoryById(id);

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
            var data = await _storyService.CreateStoryWithInitialSegment(dto ,authorId);
            return Ok(data);

        }

    }
}
