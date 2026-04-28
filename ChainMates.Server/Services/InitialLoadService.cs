using ChainMates.Server;
using ChainMates.Server.DTOs;
using ChainMates.Server.DTOs.Author;
using ChainMates.Server.DTOs.Segment;
using ChainMates.Server.DTOs.Story;
using ChainMates.Server.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using System.Diagnostics;
using System.Runtime.Intrinsics.Arm;

namespace ChainMates.Server.Services
{
    public class InitialLoadService
    {

        private readonly AppDbContext _context;
        private readonly AuthorService _authorService;
        private readonly ISegmentService _segmentService;
        public InitialLoadService(AppDbContext context, AuthorService authorService, ISegmentService segmentService)
        {
            _context = context;
            _segmentService = segmentService;
            _authorService = authorService;
        }


        public async Task<DashboardDto> getInitialLoad(int authorId)
        {
            var authorInfo = await _authorService.GetAuthorDtoById(authorId);

            // Get segments of this author's that are "in progress" (change func to take the enum not the int?)
            var activeWriteSegments = await _segmentService.GetSegmentIdsByAuthorIdAndStatusId(authorId,
                (int)SegmentStatusEnum.InProgress);
            var writeDicts = new List<SegmentHistoryDto>();
            foreach (int finalSegmentId in activeWriteSegments)
            {
                var activeSegmentHistoryDto = await _segmentService.GetSegmentHistoryBySegment(finalSegmentId);
                writeDicts.Add(activeSegmentHistoryDto);
            }

            // Get segments currently assigned to this author (and not closed)
            var activeReviewSegments = await _segmentService.GetModeratedSegmentIdsByAuthorId(authorId);
            var reviewDicts = new List<SegmentHistoryDto>();
            foreach (int finalSegmentId in activeReviewSegments)
            {
                var activeSegmentHistoryDto = await _segmentService.GetSegmentHistoryBySegment(finalSegmentId);
                reviewDicts.Add(activeSegmentHistoryDto);
            }

            // Not used for now
            var startingUrlDto = new StartingUrlDto
            {
                WriteOrReview = null,
                StoryId = null
            };

            // Pass the user's followers /followees -- this isn't being used much yet
            var authorsWhoYouFollow = await _authorService.GetAuthorsWhoYouFollow(authorId);
            var authorsWhoFollowYou = await _authorService.GetAuthorsWhoFollowYou(authorId);
            var circles = await _authorService.GetCirclesByAuthorId(authorId);

            var relationInfoDto = new RelationInfoDto
            {
                AuthorsWhoYouFollow = authorsWhoYouFollow,
                AuthorsWhoFollowYou = authorsWhoFollowYou,
                Circles = circles
            };

            var dashboardInfo = new DashboardDto
            {
                AuthorInfo = authorInfo,
                WriteDicts = writeDicts,
                ReviewDicts = reviewDicts,
                StartingUrlDict = startingUrlDto,
                RelationInfo = relationInfoDto
            };

            return dashboardInfo;

        }
    }


}