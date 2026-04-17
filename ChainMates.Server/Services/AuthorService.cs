using ChainMates.Server;
using ChainMates.Server.DTOs.Author;
using ChainMates.Server.DTOs.Segment;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using System.Text.Json;
using System.Xml.Linq;

namespace ChainMates.Server.Services
{
    public class AuthorService
    {

        private readonly AppDbContext _context;
        public AuthorService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Author>> GetAuthors()
        {
            return await _context.Author.ToListAsync();
        }

        public async Task<AuthorDto?> GetAuthorById(int authorId)
        {
            return await (from a in _context.Author
                          where a.Id == authorId
                          select new AuthorDto
                          {
                              Id = a.Id,
                              DisplayName = a.DisplayName
                          }).FirstOrDefaultAsync();
        }

        public async Task<AuthorDto?> GetAuthorDtoById(int authorID)
        {
            return await _context.Author
                .Where(a => a.Id == authorID)
                .Select(a => new AuthorDto
                {
                    Id = a.Id,
                    DisplayName = a.DisplayName
                }
                ).FirstOrDefaultAsync();
        }

        public async Task<Author> CreateAuthor(AuthorCreationDto dto)
        {
            var author = new Author
            {
                DisplayName = dto.DisplayName,
                EmailAddress = dto.EmailAddress,
                Password = dto.Password
            };
            _context.Author.Add(author);
            await _context.SaveChangesAsync();
            return author;
        }


        public async Task<List<AuthorDto>> GetFollowedAuthors(int authorId)
        {
            return await (from ar in _context.AuthorRelation
                          join a in _context.Author
                          on ar.RelatedAuthorId equals a.Id
                          where ar.AuthorRelationTypeId == 1
                          where ar.AuthorId == authorId
                          select new AuthorDto
                          {
                              Id = a.Id,
                              DisplayName = a.DisplayName
                          }
                          ).ToListAsync();
        }
        public async Task<List<AuthorDto>> GetFollowingAuthors(int authorId)
        {
            return await (from ar in _context.AuthorRelation
                          join a in _context.Author
                          on ar.AuthorId equals a.Id
                          where ar.AuthorRelationTypeId == 1
                          where ar.RelatedAuthorId == authorId
                          select new AuthorDto
                          {
                              Id = a.Id,
                              DisplayName = a.DisplayName
                          }
                          ).ToListAsync();
        }
        public async Task<AuthorRelation> FollowAuthor(int authorId, int authorToFollowId)
        {
            var authorRelation = new AuthorRelation
            {
                AuthorId = authorId,
                RelatedAuthorId = authorToFollowId,
                AuthorRelationTypeId = 1

            };
            _context.AuthorRelation.Add(authorRelation);

            await _context.SaveChangesAsync();
            return authorRelation;
        }

        public async Task<List<Circle>> GetCircles()
        {
            return await _context.Circle.ToListAsync();
        }
        
        public async Task<List<CircleDto>> GetCirclesByAuthorId(int authorId)
        {
            return await (from ca in _context.CircleAssignment
                          join c in _context.Circle on ca.CircleId equals c.Id
                          where ca.AuthorId == authorId
                          select new CircleDto
                          {
                              Id = c.Id,
                              Name = c.Name
                          }).ToListAsync();
        }

        public async Task<List<int>> GetAuthorIdsByCircleId(int circleId)
        {
            return await _context.CircleAssignment
                .Where(ca => ca.CircleId == circleId)
                .Select(ca => ca.AuthorId)
                .ToListAsync();
        }

        public async Task<Circle> CreateCircle(string name, int? authorId)
        {
            var circle = new Circle
            {
                Name = name
            };

            _context.Circle.Add(circle);
            await _context.SaveChangesAsync();

            if (authorId != null)
            {
                await JoinCircle(circle.Id, (int)authorId);
            }


            return circle;
        }

        public async Task<CircleAssignment> JoinCircle(int circleId, int authorId)
        {

            var circleAssignment = new CircleAssignment
            {
                CircleId = circleId,
                AuthorId = (int)authorId
            };
            _context.CircleAssignment.Add(circleAssignment);
            await _context.SaveChangesAsync();
            return circleAssignment;
        }

        public async Task<List<SegmentHistoryIncludingCommentsDto>> GetRecentSegmentHistoriesByAuthorId(int authorId, int numberOfSegments)
        {
            var segmentService = new SegmentService(_context);

            var segmentIdList = (from s in _context.Segment
                                     where s.SegmentStatusId == 4
                                     where s.AuthorId == authorId
                                     orderby s.Id descending
                                     select s.Id)
                              .Take(numberOfSegments);
            List<SegmentHistoryIncludingCommentsDto> dtoList = new List<SegmentHistoryIncludingCommentsDto>();
            foreach (var segmentId in segmentIdList)
            {
                var segmentTrace = await segmentService.GetSegmentTraceBySegment(segmentId);
                dtoList.Add(segmentTrace);
            }
            return dtoList;
        }
    }
}
