using Humanizer;
using Microsoft.EntityFrameworkCore;
using Mono.TextTemplating;
using NuGet.Protocol.Core.Types;
using ChainMates.Server;
using ChainMates.Server.DTOs.Author;
using ChainMates.Server.DTOs.Segment;
using ChainMates.Server.DTOs.Story;
using System;
using System.Diagnostics;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using Microsoft.EntityFrameworkCore.Query.Internal;
using ChainMates.Server.DTOs.Notification;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using System.Text.Json;
using ChainMates.Server.DTOs.Notification.Info;

namespace ChainMates.Server.Services
{
    public class NotificationService : INotificationService
    {

        private readonly AppDbContext _context;
        private readonly Random _rnd;
        public NotificationService(AppDbContext context)
        {
            _context = context;
            _rnd = new Random();
        }

        public async Task<List<NotificationDisplayDto>> GetRecentNotificationsByRecipient(int recipientId, int? numberToFetch = null, List<int>? includeTypesById = null)
        {
            includeTypesById ??= [1, 2, 3, 4, 5, 6, 7, 8, 9, 10]; //Very lazy, I'll figure out a better way later

            var query = from n in _context.Notification
                        where n.RecipientAuthorId == recipientId
                        where includeTypesById.Contains(n.NotificationTypeId)
                        orderby n.DateCreated
                        select new NotificationDisplayDto
                        {
                            Id = n.Id,
                            DateCreated = n.DateCreated,
                            NotificationTypeId = n.NotificationTypeId,
                            Info = n.Info
                        };
            if (numberToFetch != null)
            {
                query = query.Take((int)numberToFetch);
            }

            return await query.ToListAsync();
        }


        public async Task<List<Notification>> CreateNotifications(NotificationCreationDto dto, List<int> recipientIds)
        {
            // To be called by every other notification method once the recipients hav ebeen determined
            var createdNotifications = recipientIds.Select(
                recipientId => new Notification
                {
                    RecipientAuthorId = recipientId,
                    DateCreated = DateTime.UtcNow,
                    NotificationTypeId = dto.NotificationTypeId,
                    Info = JsonDocument.Parse(JsonSerializer.Serialize(dto.Info))
                }).ToList();

            await _context.Notification.AddRangeAsync(createdNotifications);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.InnerException.Message);
            }

            return createdNotifications;

        }

        public async Task<string> NotifySegmentApproved(int segmentId, int moderatorAuthorId)
        {
            // Not quite sure of the best format to pass variables in -- might be a DTO
            // involved at some point in the future
            var previousAuthorIds = await (from st in _context.SegmentTrace
                                           where st.FinalSegmentId == segmentId
                                           select st.EarlierSegmentAuthorId)
                                  .ToListAsync();
            var finalAuthorId = previousAuthorIds.FirstOrDefault();
            var finalAuthorFollowerIds = await (from ar in _context.AuthorRelation
                                                where ar.RelatedAuthorId == finalAuthorId
                                                select ar.AuthorId).ToListAsync();

            var instigator = await (from a in _context.Author
                                    where a.Id == moderatorAuthorId
                                    select new AuthorDto
                                    {
                                        Id = a.Id,
                                        DisplayName = a.DisplayName
                                    }).FirstOrDefaultAsync();

            var followedAuthor = await (from a in _context.Author
                                    where a.Id == finalAuthorId
                                        select new AuthorDto
                                    {
                                        Id = a.Id,
                                        DisplayName = a.DisplayName
                                    }).FirstOrDefaultAsync();

            var story = await (from s in _context.Segment
                               where s .Id == segmentId
                               join st in _context.Story
                               on s.StoryId equals st.Id
                               join a in _context.Author
                               on st.AuthorId equals a.Id
                               select new StoryInfoDto
                               {
                                   Id = st.Id,
                                   Title = st.Title,
                                   Author = new AuthorDto
                                   {
                                       Id = a.Id,
                                       DisplayName = a.DisplayName
                                   }
                               }).FirstOrDefaultAsync();

            await CreateNotifications(new NotificationCreationDto
            {
                NotificationTypeId = 2, //ENUM these at some point
                Info = new AuthorApprovedYourSegmentDto
                {
                    SegmentId = segmentId,
                    Instigator = instigator
                }
            }, [finalAuthorId]);

            await CreateNotifications(new NotificationCreationDto
            {
                NotificationTypeId = 3,
                Info = new AuthorYouFollowPublishedSegmentDto
                {
                    SegmentId = segmentId,
                    Instigator = instigator,
                    FollowedAuthor = followedAuthor
                }
            }, finalAuthorFollowerIds);

            await CreateNotifications(new NotificationCreationDto
            {
                NotificationTypeId = 4,
                Info = new StoryYouJoinedWasExtendedDto
                {
                    SegmentId = segmentId,
                    Story = story
                }
            }, previousAuthorIds.Skip(1).ToList());

            return "Done"; //fix later?
        }

        public async Task<string> NotifyCommentPosted(int commentTypeId, int parentId, int authorId)
        {
            int recipientId;

            var commentType = (enums.CommentType)commentTypeId;
            switch (commentType)
            { // These enums aren't the cleanest -- at some point change to enum in the DTO?
                case enums.CommentType.Story:
                    recipientId = await (from s in _context.Story
                                         where s.Id == parentId
                                         select s.AuthorId).FirstOrDefaultAsync();
                    break;
                case enums.CommentType.Segment:
                    recipientId = await (from s in _context.Segment
                                         where s.Id == parentId
                                         select s.AuthorId).FirstOrDefaultAsync();
                    break;
                case enums.CommentType.Comment:
                default:
                    recipientId = await (from s in _context.Comment
                                         where s.Id == parentId
                                         select s.AuthorId).FirstOrDefaultAsync();
                    break;
            }

            var instigator = await (from a in _context.Author
                                    where a.Id == authorId
                                    select new AuthorDto
                                    {
                                        Id = a.Id,
                                        DisplayName = a.DisplayName
                                    }).FirstOrDefaultAsync();

            await CreateNotifications(new NotificationCreationDto
            {
                NotificationTypeId = 5,
                Info = new AuthorAddedACommentDto
                {
                    CommentTypeId = commentTypeId,
                    ParentId = parentId,
                    Instigator = instigator
                }
            }, [recipientId]);

            return "Done"; //fix later?

        }

        public async Task<string> NotifyYouFollowedSomeone(int followedId, int followerId)
        {

            int recipientId = followedId;

            var instigator = await (from a in _context.Author
                                    where a.Id == followerId
                                    select new AuthorDto
                                    {
                                        Id = a.Id,
                                        DisplayName = a.DisplayName
                                    }).FirstOrDefaultAsync();

            await CreateNotifications(new NotificationCreationDto
            {
                NotificationTypeId = 1,
                Info = new AuthorFollowedYou
                {
                    Instigator = instigator
                }
            }, [recipientId]);

            return "Done"; //fix later?

        }

    }
}