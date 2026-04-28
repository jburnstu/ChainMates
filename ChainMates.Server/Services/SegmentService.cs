using Humanizer;
using Microsoft.EntityFrameworkCore;
using Mono.TextTemplating;
using NuGet.Protocol.Core.Types;
using ChainMates.Server;
using ChainMates.Server.DTOs.Author;
using ChainMates.Server.DTOs.Segment;
using ChainMates.Server.DTOs.Story;
using System;
using System.Diagnostics;
using ChainMates.Server.Enums;
using ChainMates.Server.Services.Interfaces;
using ChainMates.Server.Rules;

namespace ChainMates.Server.Services
{
    public class SegmentService : ISegmentService
    {

        private readonly AppDbContext _context;
        private readonly Random _rnd;

        private readonly ICommentService _commentService;
        private readonly INotificationService _notificationService;

        private readonly ISegmentRules _segmentRules;
        public SegmentService(AppDbContext context, ICommentService commentService, INotificationService notificationService, ISegmentRules segmentRules)
        {
            _context = context;
            _rnd = new Random();
            _commentService = commentService;
            _notificationService = notificationService;
            _segmentRules = segmentRules;
        }

        public async Task<Segment> GetSegment(int segmentId)
        {
            return await _context.Segment
                .SingleAsync(s => s.Id == segmentId);

        }

        public async Task<List<int>> GetSegmentIdsByAuthorIdAndStatusId(int authorId, int segmentStatusId)
        //Revisit naming of functions like this -- do we need "Id" after everything?
        {
            return await (from s in _context.Segment
                          where s.AuthorId == authorId
                          where s.SegmentStatusId == segmentStatusId
                          select s.Id
                          ).ToListAsync();
        }


        public async Task<Story> GetStoryBySegment(int segmentId)
        {
            Debug.WriteLine("in get story by segment");
            var story = await _context.Story
                .Where(s => s.Segments.Any(seg => seg.Id == segmentId))
                .FirstOrDefaultAsync();
            Debug.WriteLine(story.Id);
            return story;
        }

        public async Task<List<int>> GetModeratedSegmentIdsByAuthorId(int authorId)
        {
            return await (from ma in _context.ModerationAssignment
                          where ma.AuthorId == authorId
                          where !ma.IsClosed 
                          select ma.SegmentId
                          ).ToListAsync();
        }



        public async Task<HistoricalSegmentDto?> GetHistoricalSegment(int segmentId)
            // The DTO that is nested inside another segment's info when its history is needed
        {
            var childComments = await _commentService.GetHistoricalSegmentCommentAndChildren(segmentId);

            return await _context.Segment
                .Where(st => st.Id == segmentId)
                .Select(st => new HistoricalSegmentDto
                {
                    Id = st.Id,
                    Content = st.Content,
                    Author = new AuthorDto
                    {
                        Id = st.AuthorId,
                        DisplayName = st.Author.DisplayName
                    },
                    ChildComments = childComments
                }
                ).FirstOrDefaultAsync();
        }

        public async Task<SegmentHistoryDto?> GetSegmentHistoryBySegment(int segmentId)
        {
            var story = await GetStoryBySegment(segmentId);

            var storyComments = await _commentService.GetStoryCommentAndChildrenForHistory(story.Id);
            var storyDto = new StoryIncludingCommentsDto
            {
                Title = story.Title,
                MaxSegments = story.MaxSegments,
                MaxSegmentLength = story.MaxSegmentLength,
                MinSegmentLength = story.MinSegmentLength,
                MaxBranches = story.MaxBranches,
                IsItMature = story.IsItMature,
                ChildComments = storyComments
            };

            List<HistoricalSegmentDto> segmentHistoryList = new List<HistoricalSegmentDto>();

            List<int> earlierSegmentIdList = await _context.SegmentTrace
                .Where(st => st.FinalSegmentId == segmentId)
                .Select(st => st.EarlierSegmentId).ToListAsync();

            foreach (int earlierSegmentId in earlierSegmentIdList)
            {
                HistoricalSegmentDto segmentDto = await GetHistoricalSegment(earlierSegmentId);
                segmentHistoryList.Add(segmentDto);
            }


            return new SegmentHistoryDto
            {
                Id = segmentId,
                StoryData = storyDto,
                SegmentHistoryList = segmentHistoryList
            };


        }

        public async Task<Segment> CreateSegment(SegmentCreationDto dto, int authorId, bool save)
        {
            var storyId = dto.StoryId ?? 0;
            if (dto.PreviousSegmentId != null) // lock previous segment until this one has been approved
            {

                Segment? previousSegment = await (from s in _context.Segment
                                                  where s.Id == dto.PreviousSegmentId
                                                  select s).FirstOrDefaultAsync();
                previousSegment.SegmentStatusId = (int)SegmentStatusEnum.LockedForAddition;
                storyId = previousSegment.StoryId;

            }

            var segment = new Segment
            {
                AuthorId = authorId,
                StoryId = storyId,
                SegmentStatusId = dto.SegmentStatusId ?? (int)SegmentStatusEnum.InProgress,
                PreviousSegmentId = dto.PreviousSegmentId,
                Content = (dto.Content ?? "").ToString()
            };

                _context.Segment.Add(segment);
            if (save == true) // From when I was bulk-calling this method -- will delete soon if not reintroduced
            {
                await _context.SaveChangesAsync();
            }
            return segment;
        }

        public async Task<string> UpdateSegmentContent(int segmentId, string content)
        {
            var segment = await GetSegment(segmentId);
            segment.Content = content;
            await _context.SaveChangesAsync();
            return content;
        }
        
        public async Task<string> SubmitSegmentForModeration(int segmentId, string content)
        {
            var segment = await GetSegment(segmentId);
            segment.SegmentStatusId = (int)SegmentStatusEnum.AvailableForModeration;
            segment.Content = content;
            await _context.SaveChangesAsync();
            return "Done!";
        }


        public async Task<ModerationAssignment> CreateModerationAssignment(int segmentId, int authorId)
        {

            var moderationAssignment = new ModerationAssignment
            {
                AuthorId = authorId,
                SegmentId = segmentId,
                IsClosed = false
            };
            _context.ModerationAssignment.Add(moderationAssignment);

            var segment = await GetSegment(segmentId);
            // Currently moderation is tracked both by moderationassignment objects and by segmentstatusid.
            // This isn't ideal, but it's slightly for posterity in case segments need to be reviewed
            // multiple times in the future.
            segment.SegmentStatusId = (int)SegmentStatusEnum.LockedForModeration;

            await _context.SaveChangesAsync();
            return moderationAssignment;

        }
        
        public async Task<int> ApproveModeration(int segmentId, int authorId)
        {
            var moderationAssignment = await (from ma in _context.ModerationAssignment
                                        where ma.SegmentId == segmentId
                                        where ma.AuthorId == authorId
                                        select ma).FirstOrDefaultAsync();

            moderationAssignment.IsClosed = true;

            // Set the segment as available for addition (ie approved)
            Segment segment = await (from s in _context.Segment
                                     where s.Id == moderationAssignment.SegmentId
                                     select s).FirstOrDefaultAsync();
            segment.SegmentStatusId = (int)SegmentStatusEnum.AvailableForAddition;

            // Same for previous segment if exists
            Segment? previousSegment = await (from s in _context.Segment
                                         where s.Id == moderationAssignment.SegmentId
                                         select s.PreviousSegment)
                                         .FirstOrDefaultAsync();
            if (previousSegment != null)
            {
                previousSegment.SegmentStatusId = (int)SegmentStatusEnum.AvailableForAddition;
            }

            await _notificationService.NotifySegmentApproved(segmentId, authorId);
            await _context.SaveChangesAsync();
            return segmentId;
        }

        public async Task<string> AbandonSegment(int segmentId, string content)
        {
            var segment = await GetSegment(segmentId);
            segment.SegmentStatusId = (int)SegmentStatusEnum.Abandoned;
            segment.Content = content;

            // If there's a previous segment, it becomes available again
            if (segment.PreviousSegmentId != null)
            {
                Segment? previousSegment = await GetSegment((int)segment.PreviousSegmentId);
                previousSegment?.SegmentStatusId = (int)SegmentStatusEnum.AvailableForAddition;
            }

            await _context.SaveChangesAsync();
            return "Done!";
        }


        public async Task<List<int>> GetJoinableSegmentIdsByAuthor(int authorId)
        {
            var traces = await _context.SegmentTrace.ToListAsync();
            return _segmentRules.GetJoinableSegmentIdsByAuthor(authorId, traces);
        }

        public async Task<List<int>> GetModeratableSegmentIdsByAuthor(int authorId)
        {
            var traces = await _context.SegmentTrace.ToListAsync();
            return _segmentRules.GetModeratableSegmentIdsByAuthor(authorId, traces);
        }

    }
}
