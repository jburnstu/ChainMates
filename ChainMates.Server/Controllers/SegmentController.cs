using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ChainMates.Server.DTOs.Segment;
using ChainMates.Server.Services;
using System.Diagnostics;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ChainMates.Server.Controllers
{
    [Route("chainmates/segments")]
    [ApiController]
    public class SegmentController : ControllerBase
    {

        private readonly AppDbContext _context;
        private readonly SegmentService _service;
        private readonly CurrentUserService _currentUserService;

        public SegmentController(AppDbContext context, CurrentUserService currentUserService)
        {
            Debug.WriteLine("in service constructor");
            _context = context;
            _service = new SegmentService(context);
            _currentUserService = currentUserService;
        }



        // GET: api/segments
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var data = _service.GetSegments();
            return Ok(data);
        }

        // GET api/segments/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var data = await _service.GetSegmentById(id);

            return Ok(data);
        }

        // POST api/segments
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
        

        // PATCH api/segments/5
        [Authorize]
        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchAsync(int id, [FromBody] SegmentPatchDto dto)
        {

            var segment = await _service.GetSegmentById(id);
            var data = await _service.UpdateSegment(segment, dto);
            return Ok(data);

        }

        // PATCH api/segments/5
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

        // POST api/moderationassignments/
        [Authorize]
        [HttpPost("moderationassignments")]

        public async Task<IActionResult> PostModerationAssignmentAsync([FromBody] ModerationAssignmentDto dto)
        {
            int authorId = _currentUserService.UserId ?? 0;

            if (authorId == 0)
            {
                return Unauthorized();
            }
            await _service.CreateModerationAssignment(dto, authorId);
            return Ok(dto);

        }



        //GET api/segments/traces/5

        [HttpGet("traces/{idForTrace}")]
        public async Task<IActionResult> GetSegmentTraces(int idForTrace)
        {
            var data = await _service.GetSegmentTraceBySegment(idForTrace);
            return Ok(data);
        }

    }
}
