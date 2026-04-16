using ChainMates.Server;
using ChainMates.Server.DTOs.Author;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using System.Text.Json;
using System.Xml.Linq;

namespace ChainMates.Server.Services
{
    public class NOTUSEDCircleService
    {

        private readonly AppDbContext _context;
        public NOTUSEDCircleService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Author>> GetAuthors()
        {
            return await _context.Author.ToListAsync();
        }

        public async Task<Author> GetAuthorById(int authorId)
        {
            return await _context.Author
                .SingleAsync(a => a.Id == authorId);
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


        public async Task<List<int>> GetFollowedAuthors(int authorId)
        {
            var followedAuthors = await _context.AuthorRelation
                .Where(ar => ar.AuthorId == authorId)
                .Where(ar => ar.AuthorRelationTypeId == 1)
                .Select(ar =>ar.RelatedAuthorId)
                .ToListAsync();

            return followedAuthors;
        }
        public async Task<List<int>> GetFollowingAuthors(int authorId)
        {
            var followedAuthors = await _context.AuthorRelation
                .Where(ar => ar.RelatedAuthorId == authorId)
                .Where(ar => ar.AuthorRelationTypeId == 1)
                .Select(ar => ar.AuthorId)
                .ToListAsync();

            return followedAuthors;
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
    }
}
