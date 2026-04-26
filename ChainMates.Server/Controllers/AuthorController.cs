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
        private readonly int numberOfRecentSegments = 3;
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
        [HttpGet("whoyoufollow")]
        public async Task<IActionResult> GetAuthorsWhoYouFollow()
        {
            int authorId = _currentUserService.UserId ?? 0;

            if (authorId == 0)
            {
                return Unauthorized();
            }

            var data = await _authorService.GetAuthorsWhoYouFollow(authorId);
            return Ok(data);
        }

        [Authorize]
        [HttpGet("whofollowyou")]
        public async Task<IActionResult> GetFollowed()
        {
            int authorId = _currentUserService.UserId ?? 0;

            if (authorId == 0)
            {
                return Unauthorized();
            }

            var data = await _authorService.GetAuthorsWhoFollowYou(authorId);
            return Ok(data);
        }

        [Authorize]
        [HttpPost("whoyoufollow/{authorToFollowId}")]
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

        [Authorize]
        [HttpDelete("whoyoufollow/{authorToUnFollowId}")]
        public async Task<IActionResult> DeleteFollowingRelation(int authorToUnFollowId)
        {
            int authorId = _currentUserService.UserId ?? 0;

            if (authorId == 0)
            {
                return Unauthorized();
            }

            var data = await _authorService.UnFollowAuthor(authorId, authorToUnFollowId);
            return Ok(data);
        }

        [Authorize]
        [HttpGet("{authorId}/recentsegments")]
        public async Task<IActionResult> GetRecentSegmentsByAuthor(int authorId)
        {
            var data = await _authorService.GetRecentSegmentHistoriesByAuthorId(authorId, numberOfRecentSegments);
            return Ok(data);
        }


        // Circle endpoints not currently used
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
            var data = await _authorService.GetCirclesByAuthorId(authorId);
            return Ok(data);
        }




    }
}
