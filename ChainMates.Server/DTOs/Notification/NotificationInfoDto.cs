using System.Text.Json;

namespace ChainMates.Server.DTOs.Notification.Info
{

    abstract public class NotificationInfoDto { }

    public class AuthorApprovedYourSegmentDto : NotificationInfoDto
    {
        public int SegmentId { get; set; }
        public int ModeratorAuthorId { get; set; }

    }

    public class StoryYouJoinedWasExtendedDto : NotificationInfoDto
    {
        public int StoryId { get; set; }

    }

    public class AuthorYouFollowPublishedSegmentDto : NotificationInfoDto
    {
        public int SegmentId { get; set; }

        public int AuthorId { get; set; }
        public int ModeratorAuthorId { get; set; }

    }

    public class AuthorCommentedOnYourStoryDto : NotificationInfoDto
    {
        public int StoryId { get; set; }
        public int AuthorId { get; set; }
    }

    public class AuthorCommentedOnYourSegmentDto : NotificationInfoDto
    {
        public int SegmentId { get; set; }
        public int AuthorId { get; set; }
    }

    public class AuthorCommentedOnYourCommentDto : NotificationInfoDto
    {
        public int CommentId { get; set; }
        public int AuthorId { get; set; }
    }


}
