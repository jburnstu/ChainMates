using System.Reflection.Emit;
using System.Text.Json;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using ReactApp1.Server;
using ReactApp1.Server.DTOs.Author;

namespace ReactApp1.Server.Services
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


    }
}
