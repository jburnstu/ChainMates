using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using ChainMates.Server;
using ChainMates.Server.DTOs.Story;
using System.Diagnostics;
using ChainMates.Server.DTOs.Author;

namespace ChainMates.Server.Services
{
    public class StoryService
    {

        private readonly AppDbContext _context;
        public StoryService(AppDbContext context)
        {
            _context = context;
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

        public async Task<StoryInfoDto?> GetStoryById(int storyId)
        {
            return await (from s in _context.Story
                          where s.Id == storyId
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
            Debug.WriteLine("story.Id");
            Debug.WriteLine(story.Id);
            return story;
        }

        public async Task<Segment> CreateStoryWithInitialSegment(StoryDto storyDto, int authorId)
        {
            var story = await CreateStory(storyDto, authorId);

            SegmentService segmentService = new SegmentService(_context);
            var initialSegment = await segmentService.CreateSegment(new DTOs.Segment.SegmentCreationDto
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
    }
}
