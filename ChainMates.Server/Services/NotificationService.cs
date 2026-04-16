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
            includeTypesById ??= [1,2,3,4,5,6,7,8,9,10]; //Very lazy, I'll figure out a better way later

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

        public async Task<List<int>> GetNotificationRecipientsFromInfo(int notificationTypeId, JsonDocument info)
        {
            List<int> recipientIds = new List<int>();
            switch (notificationTypeId)
            {
                case 1: //Someone followed you
                    recipientIds.Add(info.followedId);
                    break;
                case 2: //Your segment is approved
                    recipientIds.Add(info.segmentAuthorId);
                    break;
                case 3: //Someone you follow published a segment
                    recipientIds.Add(await (from ar in _context.AuthorRelation
                                            where ar.RelatedAuthorId == info.authorId
                                            select ar.AuthorId).ToListAsync());
                    break;
            }
            return recipientIds;
        }
        

        public async Task<List<Notification>> CreateNotifications(NotificationCreationDto dto)
        {
            var recipientIds = await GetNotificationRecipientsFromInfo(dto.NotificationTypeId, dto.Info);

            var createdNotifications = new List<Notification>();
            foreach (int recipientId in recipientIds)
            {
                var notification = new Notification
                {
                    RecipientAuthorId = recipientId,
                    DateCreated = DateTime.Now,
                    NotificationTypeId = dto.NotificationTypeId,
                    Info = dto.Info
                };
                _context.Notification.Add(notification);
                createdNotifications.Add(notification);
            }
            _context.SaveChangesAsync();
            return createdNotifications;

           
        }

    }

}