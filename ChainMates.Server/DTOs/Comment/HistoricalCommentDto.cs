namespace ChainMates.Server.DTOs.Comment
{
    public class HistoricalCommentDto
    {

        public int Id { get; set; }
        public required  int CommentTypeId { get; set; }

        public string DisplayName { get; set; }
        public string Content { get; set; }
        public List<HistoricalCommentDto> ChildComments { get; set; } = new List<HistoricalCommentDto>();
        
    }
}