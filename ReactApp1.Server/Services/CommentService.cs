using Humanizer;
using Microsoft.EntityFrameworkCore;
using Mono.TextTemplating;
using ReactApp1.Server;
using ReactApp1.Server.DTOs;
using System;
using System.Diagnostics;


namespace ReactApp1.Server.Services
{
    public class CommentService
    {

        private readonly AppDbContext _context;
        private readonly Random _rnd;
        public CommentService(AppDbContext context)
        {
            _context = context;
            _rnd = new Random();
        }

        public async Task<List<Comment>> GetComments()
        {
            return await _context.Comment.ToListAsync();
        }

        public async Task<Comment> GetCommentById(int commentId)
        {
            return await _context.Comment
                .SingleAsync(c => c.Id == commentId);

        }

        public Task<Comment> CreateComment(CommentCreationDto dto, int authorId)
        {

        }
    }
}
