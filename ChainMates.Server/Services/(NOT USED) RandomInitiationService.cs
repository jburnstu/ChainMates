//using ChainMates.Server.DTOs;
//using ChainMates.Server.DTOs.Author;
//using ChainMates.Server.DTOs.Segment;
//using Microsoft.AspNetCore.Http.HttpResults;
//using System.Diagnostics;

//namespace ChainMates.Server.Services
//{
//    public class RandomInitiationService
//    {
//        private readonly AppDbContext _context;
//        private readonly SegmentService _segmentService;
//        private readonly AuthorService _authorService;
//        public RandomInitiationService(AppDbContext context)
//        {
//            Debug.WriteLine("in service constructor");
//            _context = context;
//            _segmentService = new SegmentService(_context);
//            _authorService = new AuthorService(_context);
            
//        }



//        public async Task<Segment> CreateNewSegmentByAuthor(int authorId)
//        {
//            Debug.WriteLine("CreateNewSegmentByAuthor");
//            Segment segment;
//            int? previousSegmentId;
//            var query = _context.JoinableSegmentByAuthor
//               .Where(jsba => jsba.AuthorId == authorId)
//               .Select(jsba => jsba.SegmentId);

//            var count = await query.CountAsync();
//            if (count == 0)
//            {
//                Debug.WriteLine("Count 0 -- new story");
//                StoryService storyService = new StoryService(_context);
//                var newStory = await storyService.CreateRandomStory(authorId);
//                segment = await CreateSegment(new SegmentCreationDto
//                {
//                    StoryId = newStory.Id,
//                    SegmentStatusId = 1,
//                    PreviousSegmentId = null,
//                    Content = ""
//                },
//                authorId, false); //check this
//            }
//            else
//            {
//                Debug.WriteLine("count non-zero");
//                previousSegmentId = await query
//                    .Skip(_rnd.Next(count))
//                    .FirstOrDefaultAsync();
//                Debug.WriteLine("building off previousSegmentId", previousSegmentId);
//                var previousSegment = await _context.Segment
//                    .SingleAsync(s => s.Id == previousSegmentId);

//                segment = await CreateSegment(new SegmentCreationDto
//                {
//                    StoryId = previousSegment.StoryId,
//                    SegmentStatusId = 1,
//                    PreviousSegmentId = previousSegmentId,
//                    Content = ""
//                },
//                authorId, false);
//            }
//            return segment;
//        }

//        public async Task<Segment> EditSegmentContent(Segment segment, string content)
//        {
//            Debug.WriteLine("EditSegmentContent");
//            segment.Content = content;
//            //await _context.SaveChangesAsync();
//            return segment;
//        }

//        public async Task<Segment> SubmitSegmentForModeration(Segment segment)
//        {
//            Debug.WriteLine("SubmitSegmentForModeration");
//            segment.SegmentStatusId = 2;
//            //await _context.SaveChangesAsync();
//            return segment;
//        }

//        public async Task<ModerationAssignment> FindAndAssignModerator(Segment segment)
//        {
//            Debug.WriteLine("FindAndAssignModerator");

//            var query = _context.ModeratableSegmentByAuthor
//                .Where(msba => msba.SegmentId == segment.Id)
//                .Select(msba => msba.AuthorId);

//            var count = query.Count();
//            if (count == 0)
//            {
//                throw new Exception("No available moderator found for this segment.");
//            }
//            else
//            {
//                int randomAvailableModeratorId = await query
//                    .Skip(_rnd.Next(count))
//                    .FirstOrDefaultAsync();

//                var moderationAssignment = await CreateModerationAssignment(new ModerationAssignmentDto
//                {
//                    //AuthorId = randomAvailableModeratorId,
//                    SegmentId = segment.Id,
//                },
//                randomAvailableModeratorId);

//                return moderationAssignment;
//            }
//        }


//        public async Task<Segment> ApproveModeration(ModerationAssignmentDto dto, int authorId)
//        {

//            var segment = await (from s in _context.Segment
//                                 where s.Id == dto.SegmentId
//                                 select s).FirstOrDefaultAsync();
//            segment.SegmentStatusId = 4;
//            if (segment.PreviousSegmentId is not null)
//            {
//                segment.PreviousSegment.SegmentStatusId = 4;
//            }
//            var moderationAssignment = await (from ma in _context.ModerationAssignment
//                                              where ma.AuthorId == authorId
//                                              where ma.SegmentId == dto.SegmentId
//                                              select ma)
//                                          .FirstOrDefaultAsync();
//            moderationAssignment.IsClosed = true;
//            await _context.SaveChangesAsync();
//            return segment;
//        }

//        public async Task<Segment?> CreateSubmitAndApproveSegment(int authorId, string content)
//        {
//            try
//            {
//                Debug.WriteLine("CreateSubmitAndApproveSegment");
//                Segment segment = await CreateNewSegmentByAuthor(authorId);
//                segment = await EditSegmentContent(segment, content);
//                segment = await SubmitSegmentForModeration(segment);
//                ModerationAssignment moderationAssignment = await FindAndAssignModerator(segment);
//                int moderatedSegmentId = await ApproveModeration(segment.Id, authorId);
//                await _context.SaveChangesAsync();
//                var moderatedSegment = await (from s in _context.Segment
//                                              where s.Id == moderatedSegmentId
//                                              select s).FirstOrDefaultAsync();

//                return moderatedSegment;
//            }
//            catch (Exception ex)
//            {
//                Debug.WriteLine($"Error in CreateSubmitAndApproveSegment: {ex.Message}");
//                return null;
//            }

//        }





//        public async Task CreateRandomSegmentTree(List<int> authorIds, int numberOfSegments, int segmentCharLength=200)
//        {
//            Random rnd = new Random();
//            for (int i = 0; i < numberOfSegments; i++)
//            {
//                int authorId = authorIds
//                    .OrderBy(x => rnd.Next())
//                    .FirstOrDefault();
//                string content = Guid.NewGuid().ToString();
//                await _segmentService.CreateSubmitAndApproveSegment(authorId, content);

//            }

//        }

//        public async Task<Author> CreateRandomAuthor()
//        {
//            string randomString = Guid.NewGuid().ToString();
//            Author author = await _authorService.CreateAuthor(new AuthorCreationDto
//            {
//                DisplayName = $"Author {randomString}",
//                EmailAddress = $"{randomString}@example.com",
//                Password = randomString
//            });

//            return author;
//        }


//        public async Task CreateRandomAuthorsAndSegments(RandomCreationDto dto)
//        {
//            Debug.WriteLine("In createRandomeAuthorsAndSegments");

//            List<int> authorIds = new List<int>();
//            for (int i = 0; i < dto.NumberOfAuthors; i++)
//            {
//                Author author = await CreateRandomAuthor();
//                authorIds.Add(author.Id);
//            }
//            await CreateRandomSegmentTree(authorIds, dto.NumberOfSegments, dto.SegmentCharLength);
//            await _context.SaveChangesAsync();  
//        }

//    }
//}
