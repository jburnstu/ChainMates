namespace ReactApp1.Server.DTOs.Comment
{
    public class CommentForTraceDto
    {
        public required  int CommentTypeId { get; set; }

        public string DisplayName { get; set; }
        public string Content { get; set; }
        public List<CommentForTraceDto> ChildComments { get; set; } = new List<CommentForTraceDto>();
        
    }
}