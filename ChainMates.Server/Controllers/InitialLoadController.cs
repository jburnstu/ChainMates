using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ChainMates.Server;
using ChainMates.Server.DTOs;
using ChainMates.Server.DTOs.Author;
using ChainMates.Server.DTOs.Segment;
using ChainMates.Server.Services;
using System.Reflection.Emit;
using System.Security.Claims;
using System.Text.Json;
using System.Xml.Linq;

[ApiController]
[Route("chainmates/load")]
public class InitialLoadController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly CurrentUserService _currentUserService;
    protected AuthorService authorService;
    protected SegmentService segmentService;
    protected StoryService storyService;
    protected List<SegmentHistoryIncludingCommentsDto> writeDicts;
    protected List<SegmentHistoryIncludingCommentsDto> reviewDicts;
    protected AuthorDto authorInfo;
    protected StartingUrlDto startingUrlDto;
    protected RelationInfoDto relationInfoDto;
    public DashboardDto dashboardInfo;

    public InitialLoadController(AppDbContext context, CurrentUserService currentUserService)
    {
         _context = context;
        _currentUserService = currentUserService;
        authorService = new AuthorService(_context);
        storyService = new StoryService(_context);
        segmentService = new SegmentService(_context);
        writeDicts = new List<SegmentHistoryIncludingCommentsDto>();
        reviewDicts = new List<SegmentHistoryIncludingCommentsDto>();
    }


    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetInfo()
    {
        Console.WriteLine("MADE IT TO GetInfo");
        int authorId = _currentUserService.UserId ?? 0;

        if (authorId == 0)
        {
            return Unauthorized();
        }

        authorInfo = await authorService.GetAuthorDtoById(authorId);

        var activeWriteSegments = await segmentService.GetSegmentIdsByAuthorIdAndStatusId(authorId, 1);

        foreach (int finalSegmentId in activeWriteSegments)
        {
            var activeSegmentHistoryDto = await segmentService.GetSegmentTraceBySegment(finalSegmentId);

            writeDicts.Add(activeSegmentHistoryDto);
        }

        var activeReviewSegments = await segmentService.GetModeratedSegmentIdsByAuthorId(authorId);

        foreach (int finalSegmentId in activeReviewSegments)
        {
            var activeSegmentHistoryDto = await segmentService.GetSegmentTraceBySegment(finalSegmentId);
            reviewDicts.Add(activeSegmentHistoryDto);
        }

        startingUrlDto = new StartingUrlDto
        {
            WriteOrReview = null,
            StoryId = null
        };

        var authorsWhoYouFollow = await authorService.GetAuthorsWhoYouFollow(authorId);
        var authorsWhoFollowYou = await authorService.GetAuthorsWhoFollowYou(authorId);
        var circles = await authorService.GetCirclesByAuthorId(authorId);

        relationInfoDto = new RelationInfoDto
        {
            AuthorsWhoYouFollow = authorsWhoYouFollow,
            AuthorsWhoFollowYou = authorsWhoFollowYou,
            Circles = circles
        };

        dashboardInfo = new DashboardDto
        {
            AuthorInfo = authorInfo,
            WriteDicts = writeDicts,
            ReviewDicts = reviewDicts,
            StartingUrlDict = startingUrlDto,
            RelationInfo = relationInfoDto
        };

        return Ok(dashboardInfo);

    }
}