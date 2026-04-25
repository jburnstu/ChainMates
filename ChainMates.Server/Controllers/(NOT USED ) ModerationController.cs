//using Humanizer;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using ChainMates.Server.DTOs.Segment;
//using ChainMates.Server.Services;
//using System.Diagnostics;

//// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

//namespace ChainMates.Server.Controllers
//{
//    [Route("chainmates/moderation")]
//    [ApiController]
//    public class ModerationController : ControllerBase
//    {

//        private readonly AppDbContext _context;
//        private readonly SegmentService _storyService;
//        private readonly CurrentUserService _currentUserService;

//        public ModerationController(AppDbContext context, CurrentUserService currentUserService)
//        {
//            Debug.WriteLine("in service constructor");
//            _context = context;
//            _storyService = new SegmentService(context);
//            _currentUserService = currentUserService;
//        }




//        //[Authorize]
//        //[HttpGet("segments")]

//        //public async Task<IActionResult> GetModeratableSegments()
//        //{
//        //    int authorId = _currentUserService.UserId ?? 0;

//        //    if (authorId == 0)
//        //    {
//        //        return Unauthorized();
//        //    }
//        //    var traces = await _storyService.GetSegmentTraces();
//        //    var data = _storyService.GetModeratableSegmentIdsByAuthor(authorId, traces);
//        //    return Ok(data);

//        //}

//        // POST api/moderationassignments/
//        //[Authorize]
//        //[HttpPost("assignments")]

//        //public async Task<IActionResult> PostModerationAssignmentAsync([FromBody] ModerationAssignmentDto dto)
//        //{
//        //    int authorId = _currentUserService.UserId ?? 0;

//        //    if (authorId == 0)
//        //    {
//        //        return Unauthorized();
//        //    }
//        //    await _storyService.CreateModerationAssignment(dto, authorId);
//        //    return Ok(dto);

//        //}




//    }
//}
