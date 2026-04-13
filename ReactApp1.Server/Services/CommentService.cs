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

        public async Task<List<CommentForTraceDto>> GetStoryCommentAndChildrenForTrace(int storyId)
        {
            var storyComments = await (from sc in _context.StoryComment
                                       join c in _context.Comment
                                       on sc.CommentId equals c.Id
                                       join a in _context.Author
                                       on c.AuthorId equals a.Id
                                       where sc.ParentStoryId == storyId
                                       select new
                                       {
                                           CommentId = c.Id,
                                           InnerDto = new CommentForTraceDto
                                           {
                                               CommentTypeId = 1,
                                               DisplayName = a.DisplayName,
                                               Content = c.Content
                                           }
                                       }
                        ).ToListAsync();
            var listOfStoryCommentIds = storyComments.Select(sc => sc.CommentId).ToList();

            var childComments = await (from cc in _context.CommentComment
                                       join c in _context.Comment
                                       on cc.CommentId equals c.Id
                                       join a in _context.Author
                                       on c.AuthorId equals a.Id
                                       where listOfStoryCommentIds.Contains(cc.ParentCommentId)
                                       select new
                                       {
                                           ParentCommentId = cc.ParentCommentId,
                                           InnerDto = new CommentForTraceDto
                                           {
                                               CommentTypeId = 3,
                                               DisplayName = a.DisplayName,
                                               Content = c.Content
                                           }
                                       }
                            ).ToListAsync();

            var lookup = storyComments.ToDictionary(sc => sc.CommentId, sc => sc.InnerDto);
            foreach (var child in childComments) { 
                if (lookup.TryGetValue(child.ParentCommentId, out var ParentInnerDto)) 
                {
                    ParentInnerDto.ChildComments.Add(child.InnerDto);
                }
            }
            return storyComments.Select(sc => sc.InnerDto).ToList();
        }

        public async Task<List<CommentForTraceDto>> GetSegmentCommentAndChildrenForTrace(int segmentId)
        {
            var segmentComments = await (from sc in _context.SegmentComment
                                       join c in _context.Comment
                                       on sc.CommentId equals c.Id
                                       join a in _context.Author
                                       on c.AuthorId equals a.Id
                                       where sc.ParentSegmentId == segmentId
                                         select new
                                       {
                                           CommentId = c.Id,
                                           InnerDto = new CommentForTraceDto
                                           {
                                               CommentTypeId = 1,
                                               DisplayName = a.DisplayName,
                                               Content = c.Content
                                           }
                                       }
                        ).ToListAsync();
            var listOfSegmentCommentIds = segmentComments.Select(sc => sc.CommentId).ToList();

            var childComments = await (from cc in _context.CommentComment
                                       join c in _context.Comment
                                       on cc.CommentId equals c.Id
                                       join a in _context.Author
                                       on c.AuthorId equals a.Id
                                       where listOfSegmentCommentIds.Contains(cc.ParentCommentId)
                                       select new
                                       {
                                           ParentCommentId = cc.ParentCommentId,
                                           InnerDto = new CommentForTraceDto
                                           {
                                               CommentTypeId = 3,
                                               DisplayName = a.DisplayName,
                                               Content = c.Content
                                           }
                                       }
                            ).ToListAsync();

            var lookup = segmentComments.ToDictionary(sc => sc.CommentId, sc => sc.InnerDto);
            foreach (var child in childComments)
            {
                if (lookup.TryGetValue(child.ParentCommentId, out var ParentInnerDto))
                {
                    ParentInnerDto.ChildComments.Add(child.InnerDto);
                }
            }
            return segmentComments.Select(sc => sc.InnerDto).ToList();
        }


        public async Task<Comment> UpdateComment(Comment comment, CommentPatchDto dto)
        {
            comment.Content = dto.Content;
            comment.CommentStatusId = dto.CommentStatusId;
            await _context.SaveChangesAsync();
            return comment;
        }

        public async Task<Comment> CreateAndSubmitComment(CommentCreationAndSubmissionDto dto, int authorId)
        {
            var comment = await CreateComment(new CommentCreationDto
            {
                CommentTypeId = dto.CommentTypeId,
                ParentId = dto.ParentId

            }, authorId);

            await UpdateComment(comment, new CommentPatchDto
            {
                CommentStatusId = 2,
                Content = dto.Content
            });
            return comment;
        }
    }
}
