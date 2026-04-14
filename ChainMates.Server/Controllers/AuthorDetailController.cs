using Microsoft.AspNetCore.Mvc;
using ChainMates.Server.Services;

namespace ChainMates.Server.Controllers
{

    [ApiController]
    [Route("chainmates/author")]
    public class AuthorDetailController : ControllerBase
    {
        private readonly AuthorService _authorService;
        public AuthorDetailController(AuthorService authorService)
        {
            _authorService = authorService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAuthors()
        {
            var data = await _authorService.GetAuthors();
            return Ok(data);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAuthorById(int id)
        {
            var data = await _authorService.GetAuthorById(id);
            return Ok(data);
        }
    }
}
