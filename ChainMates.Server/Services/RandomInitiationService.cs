using Microsoft.AspNetCore.Http.HttpResults;
using ChainMates.Server.DTOs.Author;
using ChainMates.Server.DTOs;
using System.Diagnostics;

namespace ChainMates.Server.Services
{
    public class RandomInitiationService
    {
        private readonly AppDbContext _context;
        private readonly SegmentService _segmentService;
        private readonly AuthorService _authorService;
        public RandomInitiationService(AppDbContext context)
        {
            Debug.WriteLine("in service constructor");
            _context = context;
            _segmentService = new SegmentService(_context);
            _authorService = new AuthorService(_context);
            
        }

        public async Task CreateRandomSegmentTree(List<int> authorIds, int numberOfSegments, int segmentCharLength=200)
        {
            Random rnd = new Random();
            for (int i = 0; i < numberOfSegments; i++)
            {
                int authorId = authorIds
                    .OrderBy(x => rnd.Next())
                    .FirstOrDefault();
                string content = Guid.NewGuid().ToString();
                await _segmentService.CreateSubmitAndApproveSegment(authorId, content);

            }

        }

        public async Task<Author> CreateRandomAuthor()
        {
            string randomString = Guid.NewGuid().ToString();
            Author author = await _authorService.CreateAuthor(new AuthorCreationDto
            {
                DisplayName = $"Author {randomString}",
                EmailAddress = $"{randomString}@example.com",
                Password = randomString
            });

            return author;
        }


        public async Task CreateRandomAuthorsAndSegments(RandomCreationDto dto)
        {
            Debug.WriteLine("In createRandomeAuthorsAndSegments");

            List<int> authorIds = new List<int>();
            for (int i = 0; i < dto.NumberOfAuthors; i++)
            {
                Author author = await CreateRandomAuthor();
                authorIds.Add(author.Id);
            }
            await CreateRandomSegmentTree(authorIds, dto.NumberOfSegments, dto.SegmentCharLength);
            await _context.SaveChangesAsync();  
        }

    }
}
