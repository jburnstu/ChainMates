namespace ChainMates.Server.DTOs.Comment
{
    public class CommentPatchDto
    {
        public required string Content { get; set; }
        public required int CommentStatusId { get; set; }
    }
}
