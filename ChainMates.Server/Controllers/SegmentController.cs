using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ChainMates.Server.DTOs.Segment;
using ChainMates.Server.Services;
using System.Diagnostics;


namespace ChainMates.Server.Controllers
{
    [Route("chainmates/segments")]
    [ApiController]
    public class SegmentController : ControllerBase
    {

        private readonly AppDbContext _context;
        private readonly SegmentService _service;
        private readonly CurrentUserService _currentUserService;
        private readonly NotificationService _notificationService;

        public SegmentController(AppDbContext context, CurrentUserService currentUserService, NotificationService notificationService)
        {
            _context = context;
            _service = new SegmentService(context);
            _currentUserService = currentUserService;
            _notificationService = notificationService;
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var data = await _service.GetSegment(id);

            return Ok(data);
        }


        ///////////////// Next few endpoints handle the creation, saving, submission, and deletion of segments
        /// (i.e. all actions on the write-tab)    ////////////////////////////////////////////////////////////

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> PostAsync([FromBody] SegmentCreationDto dto)
        {

            int authorId = _currentUserService.UserId ?? 0;

            if (authorId == 0)
            {
                return Unauthorized();
            }
            Segment segment = await _service.CreateSegment(dto, authorId, true);
            return Ok(segment);

            }
        

        [Authorize]
        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchSaveAsync(int id, [FromBody] SegmentPatchDto dto)
        {

            var data = await _service.UpdateSegmentContent(id, dto.Content);
            return Ok(data);

        }

        [Authorize]
        [HttpPost("{id}/submit")]
        public async Task<IActionResult> PostSubmitAsync(int id, [FromBody] SegmentPatchDto dto)
        {
            Debug.WriteLine("in PostSubmitAsync");

            var data = await _service.SubmitSegmentForModeration(id,dto.Content);
            return Ok(data);

        }

        [Authorize]
        [HttpPost("{id}/abandon")]
        public async Task<IActionResult> PostDeleteAsync(int id, [FromBody] SegmentPatchDto dto)
        {

            var data = await _service.AbandonSegment(id, dto.Content);

            return Ok(data);

        }


        //////////////// Handle the assignment of a user to a segment as moderator, and approval   ///////////
        /////////////       (i.e. actions on review-tab). API structure here could be cleaned up.   ///////////


        [Authorize]
        [HttpPost("moderationassignments/{segmentId}")]

        public async Task<IActionResult> PostModerationAssignmentAsync(int segmentId)
        {
            int authorId = _currentUserService.UserId ?? 0;

            if (authorId == 0)
            {
                return Unauthorized();
            }
            await _service.CreateModerationAssignment(segmentId, authorId);
            return Ok("Done!"); // Nothing needed for now

        }

        [Authorize]
        [HttpPost("moderationassignments/{segmentId}/approve")]

        public async Task<IActionResult> PostModerationApprove(int segmentId)
        {
            Debug.WriteLine("In Patch Moderation");
            int authorId = _currentUserService.UserId ?? 0;

            if (authorId == 0)
            {
                return Unauthorized();
            }
            await _service.ApproveModeration(segmentId, authorId);
            await _notificationService.NotifySegmentApproved(segmentId, authorId);
            return Ok(segmentId);

        }


        /////////////// Used extensively -- delivers a DTO with all the data in that segment's chain

        [HttpGet("{idForTrace}/history")]
        public async Task<IActionResult> GetSegmentHistory(int idForTrace)
        {
            var data = await _service.GetSegmentHistoryBySegment(idForTrace);
            return Ok(data);
        }


        ////////////////////////// Endpoints for segments available per author. Could put under authors/ instead?

        [Authorize]
        [HttpGet("joinablesegments")]

        public async Task<IActionResult> GetJoinableSegments()
        {
            int authorId = _currentUserService.UserId ?? 0;

            if (authorId == 0)
            {
                return Unauthorized();
            }
            var traces = await _service.GetSegmentTraces();
            var data = _service.GetJoinableSegmentIdsByAuthor(authorId, traces);
            return Ok(data);

        }

        [Authorize]
        [HttpGet("moderatablesegments")]

        public async Task<IActionResult> GetModeratableSegments()
        {
            int authorId = _currentUserService.UserId ?? 0;

            if (authorId == 0)
            {
                return Unauthorized();
            }
            var traces = await _service.GetSegmentTraces();
            var data = _service.GetModeratableSegmentIdsByAuthor(authorId, traces);
            return Ok(data);

        }

    }
}
