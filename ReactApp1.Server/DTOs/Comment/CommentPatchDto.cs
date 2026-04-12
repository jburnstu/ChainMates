namespace ReactApp1.Server.DTOs.Comment
{
    public class CommentPatchDto
    {
        public required string Content { get; set; }
        public required int CommentStatusId { get; set; }
    }
}
