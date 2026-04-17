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
            return await _context.Segment
                .SingleAsync(s => s.Id == segmentId);

        }


        public async Task<List<Segment>> GetSegmentsByAuthorAndStatus(int authorId, int segmentStatusId)
        {
            return await _context.Segment
                .Where(s => s.AuthorId == authorId)
                .Where(s => s.SegmentStatusId == segmentStatusId)
                .ToListAsync();
        }

        public async Task<Segment> UpdateSegment(Segment segment, SegmentPatchDto dto)
        {
            segment.SegmentStatusId = dto.SegmentStatusId ?? segment.SegmentStatusId;
            segment.Content = dto.Content ?? segment.Content;
            await _context.SaveChangesAsync();
            return segment;
        }


        public async Task<SegmentForTraceIncludingCommentsDto?> GetSegmentForTraceById(int segmentId)
        {
            CommentService commentService = new CommentService(_context);
            var childComments = await commentService.GetSegmentCommentAndChildrenForTrace(segmentId);

            return await _context.Segment
                .Where(st => st.Id == segmentId)
                .Select(st => new SegmentForTraceIncludingCommentsDto
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

        public async Task<SegmentHistoryIncludingCommentsDto?> GetSegmentTraceBySegment(int segmentId)
        {
            StoryService storyService = new StoryService(_context);
            CommentService commentService = new CommentService(_context);
            var story = await storyService.GetStoryBySegment(segmentId);
            var storyComments = await commentService.GetStoryCommentAndChildrenForTrace(story.Id);
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

            List<SegmentForTraceIncludingCommentsDto> segmentHistoryList = new List<SegmentForTraceIncludingCommentsDto>();

            List<int> earlierSegmentIdList = await _context.SegmentTrace
                .Where(st => st.FinalSegmentId == segmentId)
                .Select(st => st.EarlierSegmentId).ToListAsync();

            foreach (int earlierSegmentId in earlierSegmentIdList)
            {
                SegmentForTraceIncludingCommentsDto segmentDto = await GetSegmentForTraceById(earlierSegmentId);
                segmentHistoryList.Add(segmentDto);
            }


            return new SegmentHistoryIncludingCommentsDto
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
            var segment = new Segment
            {
                AuthorId = authorId,
                StoryId = dto.StoryId,
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

        public async Task<ModerationAssignment> CreateModerationAssignment(ModerationAssignmentDto dto, int authorId)
        {
            Debug.WriteLine("CreateModerationAssignment");

            var moderationAssignment = new ModerationAssignment
            {
                AuthorId = authorId,
                SegmentId = dto.SegmentId,
                IsClosed = false
            };
            _context.ModerationAssignment.Add(moderationAssignment);


            var segment = await _context.Segment
                .SingleAsync(s => s.Id == dto.SegmentId);
            segment.SegmentStatusId = 3;

            await _context.SaveChangesAsync();
            return moderationAssignment;

        }
        public async Task<Segment> CreateNewSegmentByAuthor(int authorId)
        {
            Debug.WriteLine("CreateNewSegmentByAuthor");
            Segment segment;
            int? previousSegmentId;
            var query = _context.JoinableSegmentByAuthor
               .Where(jsba => jsba.AuthorId == authorId)
               .Select(jsba => jsba.SegmentId);

            var count = await query.CountAsync();
            if (count == 0)
            {
                Debug.WriteLine("Count 0 -- new story");
                StoryService storyService = new StoryService(_context);
                var newStory = await storyService.CreateRandomStory(authorId);
                segment = await CreateSegment(new SegmentCreationDto
                {
                    StoryId = newStory.Id,
                    SegmentStatusId = 1,
                    PreviousSegmentId = null,
                    Content = ""
                },
                authorId, false); //check this
            }
            else
            {
                Debug.WriteLine("count non-zero");
                previousSegmentId = await query
                    .Skip(_rnd.Next(count))
                    .FirstOrDefaultAsync();
                Debug.WriteLine("building off previousSegmentId", previousSegmentId);
                var previousSegment = await _context.Segment
                    .SingleAsync(s => s.Id == previousSegmentId);

                segment = await CreateSegment(new SegmentCreationDto
                {
                    StoryId = previousSegment.StoryId,
                    SegmentStatusId = 1,
                    PreviousSegmentId = previousSegmentId,
                    Content = ""
                },
                authorId, false);
            }
            return segment;
        }

        public async Task<Segment> EditSegmentContent(Segment segment, string content)
        {
            Debug.WriteLine("EditSegmentContent");
            segment.Content = content;
            //await _context.SaveChangesAsync();
            return segment;
        }

        public async Task<Segment> SubmitSegmentForModeration(Segment segment)
        {
            Debug.WriteLine("SubmitSegmentForModeration");
            segment.SegmentStatusId = 2;
            //await _context.SaveChangesAsync();
            return segment;
        }

        public async Task<ModerationAssignment> FindAndAssignModerator(Segment segment)
        {
            Debug.WriteLine("FindAndAssignModerator");

            var query = _context.ModeratableSegmentByAuthor
                .Where(msba => msba.SegmentId == segment.Id)
                .Select(msba => msba.AuthorId);

            var count = query.Count();
            if (count == 0)
            {
                throw new Exception("No available moderator found for this segment.");
            }
            else
            {
                int randomAvailableModeratorId = await query
                    .Skip(_rnd.Next(count))
                    .FirstOrDefaultAsync();

                var moderationAssignment = await CreateModerationAssignment(new ModerationAssignmentDto
                {
                    //AuthorId = randomAvailableModeratorId,
                    SegmentId = segment.Id,
                },
                randomAvailableModeratorId);

                return moderationAssignment;
            }
        }

        public async Task<Segment> ApproveModeration(ModerationAssignment moderationAssignment)
        {
            Debug.WriteLine("ApproveModeration");
            moderationAssignment.IsClosed = true;
            var segment = moderationAssignment.Segment;
            segment.SegmentStatusId = 4;
            if (segment.PreviousSegmentId is not null)
            {
                segment.PreviousSegment.SegmentStatusId = 4;
            }
            //await _context.SaveChangesAsync();
            return segment;
        }

        public async Task<Segment> ApproveModeration(ModerationAssignmentDto dto, int authorId)
        {

            var segment = await (from s in _context.Segment
                                 where s.Id == dto.SegmentId
                                 select s).FirstOrDefaultAsync();
            segment.SegmentStatusId = 4;
            if (segment.PreviousSegmentId is not null)
            {
                segment.PreviousSegment.SegmentStatusId = 4;
            }
            var moderationAssignment = await (from ma in _context.ModerationAssignment
                                              where ma.AuthorId == authorId
                                              where ma.SegmentId == dto.SegmentId
                                              select ma)
                                          .FirstOrDefaultAsync();
            moderationAssignment.IsClosed = true;
            await _context.SaveChangesAsync();
            return segment;
        }

        public async Task<Segment?> CreateSubmitAndApproveSegment(int authorId, string content)
        {
            try
            {
                Debug.WriteLine("CreateSubmitAndApproveSegment");
                Segment segment = await CreateNewSegmentByAuthor(authorId);
                segment = await EditSegmentContent(segment, content);
                segment = await SubmitSegmentForModeration(segment);
                ModerationAssignment moderationAssignment = await FindAndAssignModerator(segment);
                Segment moderatedSegment = await ApproveModeration(moderationAssignment);
                await _context.SaveChangesAsync();
                return moderatedSegment;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in CreateSubmitAndApproveSegment: {ex.Message}");
                return null;
            }

        }

        public async Task<List<SegmentTrace>> GetSegmentTraces()
        {
            return await _context.SegmentTrace.ToListAsync();
        }

        public List<int> GetJoinableSegmentIdsByAuthor(int authorId, List<SegmentTrace> traces)
        {

            var blockedSegmentIds = traces
                .Where(t => t.EarlierSegmentAuthorId == authorId)
                .Select(t => t.FinalSegmentId)
                .ToHashSet();

            return traces
                .Where(t => t.FinalSegmentStatusId == 4)
                .Select(t => t.FinalSegmentId)
                .Distinct()
                .Where(id => !blockedSegmentIds.Contains(id))
                .ToList();
                
        }

        public List<int> GetModeratableSegmentIdsByAuthor(int authorId, List<SegmentTrace> traces)
        {

            var blockedSegmentIds = traces
                .Where(t => t.EarlierSegmentAuthorId == authorId)
                .Select(t => t.FinalSegmentId)
                .ToHashSet();

            return traces
                .Where(t => t.FinalSegmentStatusId == 2)
                .Select(t => t.FinalSegmentId)
                .Distinct()
                .Where(id => !blockedSegmentIds.Contains(id))
                .ToList();

        }

    }
}
