using ChainMates.Server.DTOs.Author;
using ChainMates.Server.DTOs.Story;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ChainMates.Server.DTOs.Notification.Info
{
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
    [JsonDerivedType(typeof(AuthorFollowedYou), "author_followed_you")]
    abstract public class NotificationInfoDto { }

    public class AuthorFollowedYou : NotificationInfoDto
    {
        public AuthorDto Instigator { get; set; }
    }

    public class AuthorApprovedYourSegmentDto : NotificationInfoDto
    {
        public int SegmentId { get; set; }
        public AuthorDto Instigator { get; set; }

    }

   

    public class StoryYouJoinedWasExtendedDto : NotificationInfoDto
    {
        public StoryInfoDto Story { get; set; }
        public int SegmentId { get; set; }

    }

    public class AuthorYouFollowPublishedSegmentDto : NotificationInfoDto
    {
        public int SegmentId { get; set; }

        public AuthorDto Instigator { get; set; }

        public AuthorDto FollowedAuthor { get; set; }

    }

    public class AuthorAddedACommentDto : NotificationInfoDto
    {
        public int CommentTypeId { get; set; }
        public int ParentId { get; set; }
        public AuthorDto Instigator { get; set; }
    }
    //public class AuthorCommentedOnYourStoryDto : NotificationInfoDto
    //{
    //    public int StoryId { get; set; }
    //    public int AuthorId { get; set; }
    //}

    //public class AuthorCommentedOnYourSegmentDto : NotificationInfoDto
    //{
    //    public int SegmentId { get; set; }
    //    public int AuthorId { get; set; }
    //}

    //public class AuthorCommentedOnYourCommentDto : NotificationInfoDto
    //{
    //    public int CommentId { get; set; }
    //    public int AuthorId { get; set; }
    //}


}
