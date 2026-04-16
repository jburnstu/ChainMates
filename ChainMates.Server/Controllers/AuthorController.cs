using Microsoft.AspNetCore.Mvc;
using ChainMates.Server.Services;
using Microsoft.AspNetCore.Authorization;

namespace ChainMates.Server.Controllers
{

    [ApiController]
    [Route("chainmates/authors")]
    public class AuthorController : ControllerBase
    {
        private readonly AuthorService _authorService;
        private readonly CurrentUserService _currentUserService;
        public AuthorController(AuthorService authorService, CurrentUserService currentUserService)
        {
            _authorService = authorService;
            _currentUserService = currentUserService;
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

        [Authorize]
        [HttpGet("following")]
        public async Task<IActionResult> GetFollowing()
        {
            int authorId = _currentUserService.UserId ?? 0;

            if (authorId == 0)
            {
                return Unauthorized();
            }

            var data = await _authorService.GetFollowingAuthors(authorId);
            return Ok(data);
        }

        [Authorize]
        [HttpGet("followed")]
        public async Task<IActionResult> GetFollowed()
        {
            int authorId = _currentUserService.UserId ?? 0;

            if (authorId == 0)
            {
                return Unauthorized();
            }

            var data = await _authorService.GetFollowedAuthors(authorId);
            return Ok(data);
        }

        [Authorize]
        [HttpGet("following/{'authorToFollowId'}")]
        public async Task<IActionResult> CreateFollowingRelation(int authorToFollowId)
        {
            int authorId = _currentUserService.UserId ?? 0;

            if (authorId == 0)
            {
                return Unauthorized();
            }

            var data = await _authorService.FollowAuthor(authorId, authorToFollowId);
            return Ok(data);
        }


        [HttpGet("circles/all")]
        public async Task<IActionResult> GetCircles()
        {

            var data = await _authorService.GetCircles();
            return Ok(data);
        }

        [Authorize]
        [HttpGet("circles")]
        public async Task<IActionResult> GetCirclesByAuthor()
        {
            int authorId = _currentUserService.UserId ?? 0;

            if (authorId == 0)
            {
                return Unauthorized();
            }
            var data = await _authorService.GetCircleIdsByAuthorId(authorId);
            return Ok(data);
        }


    }
}
