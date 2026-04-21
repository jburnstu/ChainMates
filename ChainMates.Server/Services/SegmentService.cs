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

        public async Task<List<Segment>> GetSegments()
        {
            return await _context.Segment.ToListAsync();
        }

        public async Task<Segment> GetSegmentById(int segmentId)
        {
            Debug.WriteLine("In getSegmentById");
            Debug.WriteLine(segmentId);
            return await _context.Segment
                .SingleAsync(s => s.Id == segmentId);

        }


        public async Task<List<int>> GetSegmentIdsByAuthorIdAndStatusId(int authorId, int segmentStatusId)
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



        public async Task<HistoricalSegmentDto?> GetHistoricalSegmentById(int segmentId)
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
                HistoricalSegmentDto segmentDto = await GetHistoricalSegmentById(earlierSegmentId);
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
            Debug.WriteLine("CreateSegment");
            Debug.WriteLine("STORY ID:");
            Debug.WriteLine(dto.StoryId);

            var storyId = dto.StoryId ?? 0;
            if (dto.PreviousSegmentId != null)
            {

                Segment? previousSegment = await (from s in _context.Segment
                                                  where s.Id == dto.PreviousSegmentId
                                                  select s).FirstOrDefaultAsync();
                previousSegment.SegmentStatusId = 5;
                storyId = previousSegment.StoryId;

            }

            var segment = new Segment
            {
                AuthorId = authorId,
                StoryId = storyId,
                SegmentStatusId = dto.SegmentStatusId ?? 1,
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
            var segment = await GetSegmentById(segmentId);
            segment.Content = content;
            await _context.SaveChangesAsync();
            return content;
        }
        
        public async Task<string> SubmitSegmentForModeration(int segmentId, string content)
        {
            Debug.WriteLine("in SubmitSegmentForModeration");
            var segment = await GetSegmentById(segmentId);
            segment.SegmentStatusId = 2;
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
            var segment = await GetSegmentById(segmentId);
            segment.SegmentStatusId = 3;

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
            segment.SegmentStatusId = 4;


            Segment? previousSegment = await (from s in _context.Segment
                                         where s.Id == moderationAssignment.SegmentId
                                         select s.PreviousSegment)
                                         .FirstOrDefaultAsync();
            if (previousSegment != null)
            {
                previousSegment.SegmentStatusId = 4;
            }
            await _context.SaveChangesAsync();
            return segmentId;
        }
        public async Task<string> AbandonSegment(int segmentId, string content)
        {
            var segment = await GetSegmentById(segmentId);
            segment.SegmentStatusId = 6;
            segment.Content = content;
            if (segment.PreviousSegmentId != null)
            {
                Segment? previousSegment = await GetSegmentById((int)segment.PreviousSegmentId);
                previousSegment?.SegmentStatusId = 5;
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
                .Where(t => t.FinalSegmentStatusId == 4)
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
                .Where(t => t.FinalSegmentStatusId == 2)
                .Select(t => t.FinalSegmentId)
                .Distinct()
                .Where(id => !blockedSegmentIdList.Contains(id))
                .ToList();

        }

    }
}
