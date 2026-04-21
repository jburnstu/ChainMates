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
using ChainMates.Server.enums;

namespace ChainMates.Server.Services
{
    public class SegmentService
    {

        private readonly AppDbContext? _context;
        private readonly Random _rnd;
        public SegmentService(AppDbContext context)
        {
            _context = context;
            _rnd = new Random();
        }

        public SegmentService()
        {
            _rnd = new Random();
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


        public async Task<List<int>> GetModeratedSegmentIdsByAuthorId(int authorId)
        {
            return await (from ma in _context.ModerationAssignment
                          where ma.AuthorId == authorId
                          where !ma.IsClosed 
                          select ma.SegmentId
                          ).ToListAsync();
        }



        public async Task<HistoricalSegmentDto?> GetHistoricalSegment(int segmentId)
        {
            CommentService commentService = new CommentService(_context);
            var childComments = await commentService.GetHistoricalSegmentCommentAndChildren(segmentId);

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
            StoryService storyService = new StoryService(_context);
            var story = await storyService.GetStoryBySegment(segmentId);

            CommentService commentService = new CommentService(_context);
            var storyComments = await commentService.GetStoryCommentAndChildrenForHistory(story.Id);
            var storyDto = new StoryIncludingCommentsDto
            {
                //Id = story.Id,
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
            if (dto.PreviousSegmentId != null)
            {

                Segment? previousSegment = await (from s in _context.Segment
                                                  where s.Id == dto.PreviousSegmentId
                                                  select s).FirstOrDefaultAsync();
                previousSegment.SegmentStatusId = (int)enums.SegmentStatus.LockedForAddition;
                storyId = previousSegment.StoryId;

            }

            var segment = new Segment
            {
                AuthorId = authorId,
                StoryId = storyId,
                SegmentStatusId = dto.SegmentStatusId ?? (int)enums.SegmentStatus.InProgress,
                PreviousSegmentId = dto.PreviousSegmentId,
                Content = (dto.Content ?? "").ToString()
            };

                _context.Segment.Add(segment);
            if (save == true)
            {
                Debug.WriteLine("Save is true");
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
            Debug.WriteLine("in SubmitSegmentForModeration");
            var segment = await GetSegment(segmentId);
            segment.SegmentStatusId = (int)enums.SegmentStatus.AvailableForModeration;
            segment.Content = content;
            await _context.SaveChangesAsync();
            return "Done!";
        }


        public async Task<ModerationAssignment> CreateModerationAssignment(int segmentId, int authorId)
        {
            Debug.WriteLine("CreateModerationAssignment");

            var moderationAssignment = new ModerationAssignment
            {
                AuthorId = authorId,
                SegmentId = segmentId,
                IsClosed = false
            };
            _context.ModerationAssignment.Add(moderationAssignment);

            Debug.WriteLine(segmentId);
            var segment = await GetSegment(segmentId);
            segment.SegmentStatusId = (int)enums.SegmentStatus.LockedForModeration;

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

            Segment segment = await (from s in _context.Segment
                                     where s.Id == moderationAssignment.SegmentId
                                     select s).FirstOrDefaultAsync();
            segment.SegmentStatusId = (int)enums.SegmentStatus.AvailableForAddition;


            Segment? previousSegment = await (from s in _context.Segment
                                         where s.Id == moderationAssignment.SegmentId
                                         select s.PreviousSegment)
                                         .FirstOrDefaultAsync();
            if (previousSegment != null)
            {
                previousSegment.SegmentStatusId = (int)enums.SegmentStatus.AvailableForAddition;
            }
            await _context.SaveChangesAsync();
            return segmentId;
        }
        public async Task<string> AbandonSegment(int segmentId, string content)
        {
            var segment = await GetSegment(segmentId);
            segment.SegmentStatusId = (int)enums.SegmentStatus.Abandoned;
            segment.Content = content;
            if (segment.PreviousSegmentId != null)
            {
                Segment? previousSegment = await GetSegment((int)segment.PreviousSegmentId);
                previousSegment?.SegmentStatusId = (int)enums.SegmentStatus.LockedForAddition;
            }

            await _context.SaveChangesAsync();
            return "Done!";
        }



        public async Task<List<SegmentTrace>> GetSegmentTraces()
        {
        
            return await _context.SegmentTrace.ToListAsync();
        }

        public async Task<List<int>> GetJoinableSegmentIdsByAuthor(int authorId, List<SegmentTrace> traces)
        {

            var blockedSegmentIdList = traces
                .Where(t => t.EarlierSegmentAuthorId == authorId)
                .Select(t => t.FinalSegmentId)
                .ToHashSet();

            return traces
                .Where(t => t.FinalSegmentStatusId == (int)enums.SegmentStatus.AvailableForAddition)
                .Select(t => t.FinalSegmentId)
                .Distinct()
                .Where(id => !blockedSegmentIdList.Contains(id))
                .ToList();
                
        }

        public List<int> GetModeratableSegmentIdsByAuthor(int authorId, List<SegmentTrace> traces)
        {

            var blockedSegmentIdList = traces
                .Where(t => t.EarlierSegmentAuthorId == authorId)
                .Select(t => t.FinalSegmentId)
                .ToHashSet();

            return traces
                .Where(t => t.FinalSegmentStatusId == (int)enums.SegmentStatus.AvailableForModeration)
                .Select(t => t.FinalSegmentId)
                .Distinct()
                .Where(id => !blockedSegmentIdList.Contains(id))
                .ToList();

        }

    }
}
