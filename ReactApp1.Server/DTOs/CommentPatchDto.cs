namespace ReactApp1.Server.DTOs
{
    public class CommentPatchDto
    {
        public required string Content { get; set; }
        public required int CommentStatusId { get; set; }
    }
}
