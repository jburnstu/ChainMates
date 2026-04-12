using Humanizer;
using Microsoft.EntityFrameworkCore;
using Mono.TextTemplating;
using ReactApp1.Server;
using ReactApp1.Server.DTOs.Comment;
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

        public async Task<Comment> CreateComment(CommentCreationDto dto, int authorId)
        {

            var comment = new Comment
            {
                AuthorId = authorId,
                CommentTypeId = dto.CommentTypeId,
                CommentStatusId = 1,
                Content = ""
            };

            _context.Comment.Add(comment);

            switch (dto.CommentTypeId)
            {
                case 1:
                    var storyComment = new StoryComment
                    {
                        CommentId = comment.Id,
                        CommentTypeId = dto.CommentTypeId,
                        ParentStoryId = dto.ParentId
                    };
                    _context.StoryComment.Add(storyComment);
                    break;
                case 2:
                    var segmentComment = new SegmentComment
                    {
                        CommentId = comment.Id,
                        CommentTypeId = dto.CommentTypeId,
                        ParentSegmentId = dto.ParentId
                    };
                    _context.SegmentComment.Add(segmentComment);
                    break;
                case 3:
                    var commentComment = new CommentComment
                    {
                        CommentId = comment.Id,
                        CommentTypeId = dto.CommentTypeId,
                        ParentCommentId = dto.ParentId
                    };
                    _context.CommentComment.Add(commentComment);
                    break;
            }
            ;
            await _context.SaveChangesAsync();
            return comment;      
        }

        public async Task<Comment> UpdateComment(Comment comment, CommentPatchDto dto)
        {
            comment.Content = dto.Content;
            comment.CommentStatusId = dto.CommentStatusId;
            await _context.SaveChangesAsync();
            return comment;
        }
    }
}
