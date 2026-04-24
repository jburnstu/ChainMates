using ChainMates.Server.DTOs.Author;
using ChainMates.Server.DTOs.Notification;

namespace ChainMates.Server.Services
{
    public interface INotificationService
    {
        Task<List<NotificationDisplayDto>> GetRecentNotificationsByRecipient(int recipientId, int? numberToFetch = null, List<int>? includeTypesById = null);
        Task<List<Notification>> CreateNotifications(NotificationCreationDto dto, List<int> recipientIds);
        Task<string> NotifySegmentApproved(int segmentId, int moderatorAuthorId);
        Task<string> NotifyCommentPosted(int commentTypeId, int parentId, int authorId);

    }
}