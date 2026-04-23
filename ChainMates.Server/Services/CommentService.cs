using Humanizer;
using Microsoft.EntityFrameworkCore;
using Mono.TextTemplating;
using ChainMates.Server;
using ChainMates.Server.DTOs.Comment;
using System;
using System.Diagnostics;
using ChainMates.Server.enums;


namespace ChainMates.Server.Services
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

        // These generic comment getters aren't used yet because comments only ever get fed into SegmentHistoryDto for now
        public async Task<List<Comment>> GetComments()
        {
            return await _context.Comment.ToListAsync();
        }

        public async Task<Comment> GetCommentById(int commentId)
        {
            return await _context.Comment
                .SingleAsync(c => c.Id == commentId);

        }



        public async Task<int> CreateComment(CommentCreationDto dto, int authorId)
            // Note: this comment method is only ever called as part of "create and submit" --
            // in actual fact, an empty comment will always be immediately updated with 
            // content for now. I'm leaving it split out in case that changes later.
        {

            var comment = new Comment
            {
                AuthorId = authorId,
                CommentTypeId = dto.CommentTypeId,
                CommentStatusId = 1,
                Content = ""
            };

            _context.Comment.Add(comment);
            await _context.SaveChangesAsync();

            var commentType = (enums.CommentType)dto.CommentTypeId;
            switch (commentType)
            {
                case enums.CommentType.Story:
                    var storyComment = new StoryComment
                    {
                        CommentId = comment.Id,
                        CommentTypeId = dto.CommentTypeId,
                        ParentStoryId = dto.ParentId
                    };
                    await _context.StoryComment.AddAsync(storyComment);
                    break;
                case enums.CommentType.Segment:
                    var segmentComment = new SegmentComment
                    {
                        CommentId = comment.Id,
                        CommentTypeId = dto.CommentTypeId,
                        ParentSegmentId = dto.ParentId
                    };
                    await _context.SegmentComment.AddAsync(segmentComment);
                    break;
                case enums.CommentType.Comment:
                    var commentComment = new CommentComment
                    {
                        CommentId = comment.Id,
                        CommentTypeId = dto.CommentTypeId,
                        ParentCommentId = dto.ParentId
                    };
                    await _context.CommentComment.AddAsync(commentComment);
                    break;
            };

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.InnerException.Message);
            }
            return comment.Id;
        }


        public async Task<CommentPatchDto> UpdateComment(Comment comment, CommentPatchDto dto)
            // Again, always called with CreateComment (see below)
        {
            comment.Content = dto.Content;
            comment.CommentStatusId = dto.CommentStatusId;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.InnerException.Message);
            }
            return dto;
        }

        public async Task<CommentCreationAndSubmissionDto> CreateAndSubmitComment(CommentCreationAndSubmissionDto dto, int authorId)
        {
            Debug.WriteLine("In createAndSubmitComment");
            int commentId = await CreateComment(new CommentCreationDto
            {
                CommentTypeId = dto.CommentTypeId,
                ParentId = dto.ParentId

            }, authorId);

            var comment = await _context.Comment.SingleAsync(c => c.Id == commentId);

            await UpdateComment(comment, new CommentPatchDto
            {
                CommentStatusId = 2,
                Content = dto.Content
            });

            NotificationService notificationService = new NotificationService(_context);
            await notificationService.NotifyCommentPosted(dto.CommentTypeId, dto.ParentId, authorId);
            return dto;
        }


        public async Task<List<HistoricalCommentDto>> GetStoryCommentAndChildrenForHistory(int storyId)
        {
            // Preps comments for the nested DTO that's passed to the frontend
            // Note that storylevel comments aren't actually displayed yet
            var storyComments = await (from sc in _context.StoryComment
                                       join c in _context.Comment
                                       on sc.CommentId equals c.Id
                                       join a in _context.Author
                                       on c.AuthorId equals a.Id
                                       where sc.ParentStoryId == storyId
                                       select new HistoricalCommentDto
                                       {
                                           Id = c.Id,
                                           CommentTypeId = (int)enums.CommentType.Story,
                                           DisplayName = a.DisplayName,
                                           Content = c.Content
                                       }
                             ).ToListAsync();
            var listOfStoryCommentIds = storyComments.Select(sc => sc.Id).ToList();

            var childComments = await (from cc in _context.CommentComment
                                       join c in _context.Comment
                                       on cc.CommentId equals c.Id
                                       join a in _context.Author
                                       on c.AuthorId equals a.Id
                                       where listOfStoryCommentIds.Contains(cc.ParentCommentId)
                                       select new
                                       {
                                           ParentCommentId = cc.ParentCommentId,
                                           InnerDto = new HistoricalCommentDto
                                           {
                                               Id = c.Id,
                                               CommentTypeId = (int)enums.CommentType.Comment,
                                               DisplayName = a.DisplayName,
                                               Content = c.Content
                                           }
                                       }
                            ).ToListAsync();

            var lookup = storyComments.ToDictionary(sc => sc.Id, sc => sc);
            foreach (var child in childComments) { 
                if (lookup.TryGetValue(child.ParentCommentId, out var parentDto)) 
                {
                    parentDto.ChildComments.Add(child.InnerDto);
                }
            }
            return storyComments.ToList();
        }

        public async Task<List<HistoricalCommentDto>> GetHistoricalSegmentCommentAndChildren(int segmentId)
        {
            // Preps comments for the nested DTO that's passed to the frontend
            var segmentComments = await (from sc in _context.SegmentComment
                                       join c in _context.Comment
                                       on sc.CommentId equals c.Id
                                       join a in _context.Author
                                       on c.AuthorId equals a.Id
                                       where sc.ParentSegmentId == segmentId
                                       select new HistoricalCommentDto
                                           {
                                               Id = c.Id,
                                               CommentTypeId = (int)enums.CommentType.Segment,
                                               DisplayName = a.DisplayName,
                                               Content = c.Content
                                           }                                      
                        ).ToListAsync();
            var segmentCommentIdList = segmentComments.Select(sc => sc.Id).ToList();

            var childComments = await (from cc in _context.CommentComment
                                       join c in _context.Comment
                                       on cc.CommentId equals c.Id
                                       join a in _context.Author
                                       on c.AuthorId equals a.Id
                                       where segmentCommentIdList.Contains(cc.ParentCommentId)
                                       select new
                                       {
                                           ParentCommentId = cc.ParentCommentId,
                                           InnerDto = new HistoricalCommentDto
                                           {
                                               Id = c.Id,
                                               CommentTypeId = (int)enums.CommentType.Comment,
                                               DisplayName = a.DisplayName,
                                               Content = c.Content
                                           }
                                       }
                            ).ToListAsync();

            var lookup = segmentComments.ToDictionary(sc => sc.Id, sc => sc);
            foreach (var child in childComments)
            {
                if (lookup.TryGetValue(child.ParentCommentId, out var ParentInnerDto))
                {
                    ParentInnerDto.ChildComments.Add(child.InnerDto);
                }
            }
            return segmentComments.Select(sc => sc).ToList();
        }


    }
}
