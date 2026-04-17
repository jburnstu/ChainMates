using System.Text.Json;

namespace ChainMates.Server.DTOs.Notification.Info
{
    public class AuthorYouFollowPublishedSegmentOld
    {
        public int SegmentId { get; set; }

        public int AuthorId { get; set; }
        public int ModeratorAuthorId { get; set; }

    }
}
