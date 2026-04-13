namespace ReactApp1.Server.DTOs.Comment
{
    public class CommentCreationAndSubmissionDto
    {
        public required  int CommentTypeId { get; set; }
        public required int ParentId { get; set; }

        public required string Content { get; set; }
    }
}
