using Microsoft.AspNetCore.Mvc;
using ReactApp1.Server.DTOs.Story;
using ReactApp1.Server.Services;
using System.Diagnostics;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ReactApp1.Server.Controllers
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
        public async Task<IActionResult> Get()
        {
            var data = _service.GetStories();
            return Ok(data);
        }

        //// GET api/stories/5
        //[HttpGet("{id}")]
        //public async Task<IActionResult> Get(int id)
        //{
        //    var data = await _storyService.GetStoryById(id);

        //    return Ok(data);
        //}

        // POST api/<StoryController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] StoryDto dto)
        {
            int authorId = _currentUserService.UserId ?? 0;

            if (authorId == 0)
            {
                return Unauthorized();
            }
            Debug.WriteLine("INSIDE POdST");
            var data = await _service.CreateStory(dto ,authorId);
            return Ok(data);

        }

        //// PUT api/<StoryController>/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody] string value)
        //{
        //}

        //// DELETE api/<StoryController>/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}
