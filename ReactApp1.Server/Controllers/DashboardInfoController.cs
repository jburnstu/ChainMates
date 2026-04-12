using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReactApp1.Server;
using ReactApp1.Server.DTOs;
using ReactApp1.Server.DTOs.Author;
using ReactApp1.Server.DTOs.Segment;
using ReactApp1.Server.Services;
using System.Reflection.Emit;
using System.Security.Claims;
using System.Text.Json;
using System.Xml.Linq;

[ApiController]
[Route("chainmates/dashboardInfo")]
public class DashboardInfoController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly CurrentUserService _currentUserService;
    protected AuthorService authorService;
    protected SegmentService segmentService;
    protected StoryService storyService;
    protected List<SegmentHistoryDto> writeDicts;
    protected List<SegmentHistoryDto> reviewDicts;
    protected AuthorDto authorInfo;
    protected StartingUrlDto startingUrlDto;
    public DashboardDto dashboardInfo;

    public DashboardInfoController(AppDbContext context, CurrentUserService currentUserService)
    {
         _context = context;
        _currentUserService = currentUserService;
        authorService = new AuthorService(_context);
        storyService = new StoryService(_context);
        segmentService = new SegmentService(_context);
        writeDicts = new List<SegmentHistoryDto>();
        reviewDicts = new List<SegmentHistoryDto>();
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

        var activeWriteSegments = await segmentService.GetSegmentsByAuthorAndStatus(authorId, 1);

        foreach (Segment finalSegment in activeWriteSegments)
        {
            var activeSegmentHistoryDto = await segmentService.GetSegmentTraceBySegment(finalSegment.Id);

            writeDicts.Add(activeSegmentHistoryDto);
        }

        var activeReviewSegments = await segmentService.GetSegmentsByAuthorAndStatus(authorId, 3);

        foreach (Segment finalSegment in activeReviewSegments)
        {
            var activeSegmentHistoryDto = await segmentService.GetSegmentTraceBySegment(finalSegment.Id);
            reviewDicts.Add(activeSegmentHistoryDto);
        }

        startingUrlDto = new StartingUrlDto
        {
            WriteOrReview = null,
            StoryId = null
        };

        dashboardInfo = new DashboardDto
        {
            AuthorInfo = authorInfo,
            WriteDicts = writeDicts,
            ReviewDicts = reviewDicts,
            StartingUrlDict = startingUrlDto
        };

        return Ok(dashboardInfo);

    }
}

// writeDicts:
//[{"final_segment_id"::,
//    "segment_trace":[{
//                    "id"::, 
//                    "content"::, 
//                    "segment_info":{
//                                    "author":{
//                                            "id"::,
//                                            "name":display_name}
//                                            },
//                                        "moderator":{
//                                                "id"::,
//                                                "name":display_name,
//                                                "moderation_notes":notes
//                                                 },
//"comments":[{
//            "commentID"::,
//            "author":{
//                        "id"::,
//                        "name":display_name}
//                        },
//            "content"::,
//            "child_comments":[{
//                                "commentID"::,
//                                "author":{
//                                        "id"::,
//                                        "name":display_name}
//                                        },
//                                "content"::,
//                                }]
//        }]
//      "story_data":{
//                  "variousData"::,
//                   "comments":},
//  ]
