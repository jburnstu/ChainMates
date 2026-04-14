using ReactApp1.Server.DTOs.Author;
using ReactApp1.Server.DTOs.Comment;

namespace ReactApp1.Server.DTOs.Segment
{
    public class SegmentForTraceIncludingCommentsDto
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public AuthorDto Author { get; set; }

        public List<CommentForTraceDto> ChildComments { get; set; } = new List<CommentForTraceDto>();
    }
}
