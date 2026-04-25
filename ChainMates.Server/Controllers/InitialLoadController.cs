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
using ChainMates.Server.Enums;

[ApiController]
[Route("chainmates/load")]
public class InitialLoadController : ControllerBase
    // For now, I have quite a lot of actual logic going on this controller. It just made sense to me for now for it to be done
    // here, since it pulls together so many services, but realistically I think at some point this should be moved into a
    // dedicated service, called from one endpoint in this file.
{
    private readonly CurrentUserService _currentUserService;
    private readonly AuthorService _authorService;
    private readonly SegmentService _segmentService;
    protected List<SegmentHistoryDto> writeDicts;
    protected List<SegmentHistoryDto> reviewDicts;
    protected AuthorDto authorInfo;
    protected StartingUrlDto startingUrlDto;
    protected RelationInfoDto relationInfoDto;
    public DashboardDto dashboardInfo;

    public InitialLoadController(
        CurrentUserService currentUserService, 
        AuthorService authorService, 
        SegmentService segmentService
        )
    {
        _currentUserService = currentUserService;
        _authorService = authorService; 
        _segmentService = segmentService;
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

        authorInfo = await _authorService.GetAuthorDtoById(authorId);

        // Get segments of this author's that are "in progress" (change func to take the enum not the int?)
        var activeWriteSegments = await _segmentService.GetSegmentIdsByAuthorIdAndStatusId(authorId,
            (int)SegmentStatusEnum.InProgress);
        foreach (int finalSegmentId in activeWriteSegments)
        {
            var activeSegmentHistoryDto = await _segmentService.GetSegmentHistoryBySegment(finalSegmentId);
            writeDicts.Add(activeSegmentHistoryDto);
        }

        // Get segments currently assigned to this author (and not closed)
        var activeReviewSegments = await _segmentService.GetModeratedSegmentIdsByAuthorId(authorId);
        foreach (int finalSegmentId in activeReviewSegments)
        {
            var activeSegmentHistoryDto = await _segmentService.GetSegmentHistoryBySegment(finalSegmentId);
            reviewDicts.Add(activeSegmentHistoryDto);
        }

        // Not used for now
        startingUrlDto = new StartingUrlDto
        {
            WriteOrReview = null,
            StoryId = null
        };

        // Pass the user's followers /followees -- this isn't being used much yet
        var authorsWhoYouFollow = await _authorService.GetAuthorsWhoYouFollow(authorId);
        var authorsWhoFollowYou = await _authorService.GetAuthorsWhoFollowYou(authorId);
        var circles = await _authorService.GetCirclesByAuthorId(authorId);

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