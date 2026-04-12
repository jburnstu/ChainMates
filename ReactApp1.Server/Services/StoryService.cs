using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using ReactApp1.Server;
using ReactApp1.Server.DTOs;
using System.Diagnostics;

namespace ReactApp1.Server.Services
{
    public class StoryService
    {

        private readonly AppDbContext _context;
        public StoryService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Story>> GetStories()
        {
            return await _context.Story.ToListAsync();
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
