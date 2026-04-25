using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using ChainMates.Server;
using ChainMates.Server.DTOs.Story;
using System.Diagnostics;
using ChainMates.Server.DTOs.Author;
using System.Runtime.Intrinsics.Arm;

namespace ChainMates.Server.Services
{
    public class StoryService : IStoryService
    {

        private readonly AppDbContext _context;
        private readonly ISegmentService _segmentService;
        public StoryService(AppDbContext context, ISegmentService segmentService)
        {
            _context = context;
            _segmentService = segmentService;
        }

        public async Task<List<StoryInfoDto>> GetStories()
        {
            return await (from s in _context.Story
                          join a in _context.Author
                          on s.AuthorId equals a.Id
                          select new StoryInfoDto
                          {
                              Id = s.Id,
                              Author = new AuthorDto
                              {
                                  Id = a.Id,
                                  DisplayName = a.DisplayName
                              },
                              Title = s.Title
                          }).ToListAsync();
        }

        public async Task<StoryInfoIncludingStructureDto?> GetStoryById(int storyId)
        {
            var storyStructure = await GetStoryStructure(storyId);

            return await (from s in _context.Story
                          where s.Id == storyId
                          join a in _context.Author
                          on s.AuthorId equals a.Id
                          select new StoryInfoIncludingStructureDto
                          {
                              Id = s.Id,
                              Author = new AuthorDto
                              {
                                  Id = a.Id,
                                  DisplayName = a.DisplayName
                              },
                              Title = s.Title,
                              Structure = storyStructure
                          }).FirstOrDefaultAsync();
        }

        public async Task<Story> CreateStory(StoryDto dto, int authorId)
        {
            var story = new Story
            {
                AuthorId = authorId,
                Title = dto.Title,
                MaxSegments = dto.MaxSegments,
                MaxSegmentLength = dto.MaxSegmentLength,
                MinSegmentLength = dto.MinSegmentLength,
                MaxBranches = dto.MaxBranches,
                IsItMature = dto.IsItMature ?? false
            };
            _context.Story.Add(story);
            await _context.SaveChangesAsync();
            return story;
        }

        public async Task<Segment> CreateStoryWithInitialSegment(StoryDto storyDto, int authorId)
        {
            var story = await CreateStory(storyDto, authorId);

            var initialSegment = await _segmentService.CreateSegment(new DTOs.Segment.SegmentCreationDto
            {
                StoryId = story.Id
            },story.AuthorId,true);

            return initialSegment;
        }

        public async Task<Story> GetStoryBySegment (int segmentId)
        {
            var story = await _context.Story
                .Where(s => s.Segments.Any(seg => seg.Id == segmentId))
                .FirstOrDefaultAsync();
            if (story == null)
            {
                throw new Exception($"No story found for segment ID {segmentId}");
            }
            return story;
        }

        public async Task<Story> CreateRandomStory(int authorId, 
                                                    bool includeMaxSegments = false,
                                                    bool includeMaxSegmentLength = false,
                                                    bool includeMinSegmentLength = false,
                                                    bool includeMaxBranches = false,
                                                    bool includeIsItMature = false)
        { 

        //int (minMaxSegments, maxMaxSegments) = (5, 100);
                
        string randomString = Guid.NewGuid().ToString();
        var dto = new StoryDto {
            Title = randomString,
            MaxSegments = includeMaxSegments ?  0 : null,
            MinSegmentLength = includeMinSegmentLength ? 0 : null,
            MaxSegmentLength = includeMaxSegmentLength ? 0 : null,
            MaxBranches = includeMaxBranches ? 0 : null,
            IsItMature = includeIsItMature ? false : null
        };

        var story = await CreateStory(dto, authorId);
            return story;

        }

        internal async Task<Dictionary<int,List<int>>> GetStoryStructure(int storyId)
        {
            var dict = await _context.Segment
                .Where(s => s.StoryId == storyId)
                .GroupJoin(
                    _context.Segment,
                    s => s.Id,
                    s2 => s2.PreviousSegmentId,
                    (s, futureSegments) => new
                    {
                        s.Id,
                        FutureIds = futureSegments.Select(fs => fs.Id).ToList()
                    }
                )
                .ToDictionaryAsync(x => x.Id, x => x.FutureIds);

            int firstSegmentId = await _context.Segment
                .Where(s => s.StoryId == storyId && s.PreviousSegmentId == null)
                .Select(s => s.Id)
                .SingleOrDefaultAsync();

            dict[0] = new List<int> { firstSegmentId };

            return dict;

        }
    }
}
