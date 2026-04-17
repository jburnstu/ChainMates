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
    public class NotificationService
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
                            //RecipientAuthorId = n.RecipientAuthorId, //don't really need this? New DTO without it?
                            //InstigatorAuthorId = n.InstigatorAuthorId,
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

            var createdNotifications = recipientIds.Select(
                recipientId => new Notification
                {
                    RecipientAuthorId = recipientId,
                    DateCreated = DateTime.Now,
                    NotificationTypeId = dto.NotificationTypeId,
                    Info = JsonSerializer.Serialize(dto.Info)
                }).ToList();

            await _context.Notification.AddRangeAsync(createdNotifications);
            await _context.SaveChangesAsync();
            return createdNotifications;

        }



        public async Task<string> NotifySegemntApproved(int segmentId, int moderatorAuthorId)
        {

            var previousAuthorIds = await (from st in _context.SegmentTrace
                                           where st.FinalSegmentId == segmentId
                                           select st.EarlierSegmentAuthorId)
                                  .ToListAsync();
            var finalAuthorId = previousAuthorIds.FirstOrDefault();
            var finalAuthorFollowerIds = await (from ar in _context.AuthorRelation
                                                where ar.RelatedAuthorId == finalAuthorId
                                                select ar.AuthorId).ToListAsync();
            var storyId = await (from s in _context.Segment
                                 where s.Id == segmentId
                                 select s.StoryId).FirstOrDefaultAsync();


            await CreateNotifications(new NotificationCreationDto
            {
                NotificationTypeId = 2,
                Info = new AuthorApprovedYourSegmentDto
                {
                    SegmentId = segmentId,
                    ModeratorAuthorId = moderatorAuthorId
                }
            }, [finalAuthorId]);

            await CreateNotifications(new NotificationCreationDto
            {
                NotificationTypeId = 3,
                Info = new AuthorYouFollowPublishedSegmentDto
                {
                    SegmentId = segmentId,
                    ModeratorAuthorId = moderatorAuthorId
                }
            }, finalAuthorFollowerIds);

            await CreateNotifications(new NotificationCreationDto
            {
                NotificationTypeId = 4,
                Info = new StoryYouJoinedWasExtendedDto
                {
                    StoryId = storyId,
                }
            }, previousAuthorIds.Skip(1).ToList());

            return "Done"; //fix later?
        }

        public async Task<string> NotifyCommentPosted(int commentTypeId, int parentId, int authorId)
        {
            int recipientId;
            NotificationInfoDto dto;
            switch (commentTypeId)
            {
                case 1:
                    recipientId = await (from s in _context.Story
                                         where s.Id == parentId
                                         select s.AuthorId).FirstOrDefaultAsync();
                    dto = new AuthorCommentedOnYourStoryDto
                    {
                        StoryId = parentId,
                        AuthorId = authorId
                    };
                    break;
                case 2:
                    recipientId = await (from s in _context.Segment
                                         where s.Id == parentId
                                         select s.AuthorId).FirstOrDefaultAsync();
                    dto = new AuthorCommentedOnYourSegmentDto
                    {
                        SegmentId = parentId,
                        AuthorId = authorId
                    };
                    break;
                case 3:
                default:
                    recipientId = await (from s in _context.Comment
                                         where s.Id == parentId
                                         select s.AuthorId).FirstOrDefaultAsync();
                    dto = new AuthorCommentedOnYourCommentDto
                    {
                        CommentId = parentId,
                        AuthorId = authorId
                    };
                    break;
            }

            await CreateNotifications(new NotificationCreationDto
            {
                NotificationTypeId = 4,
                Info = dto
            }, [recipientId]);

            return "Done"; //fix later?

        }

    }
}