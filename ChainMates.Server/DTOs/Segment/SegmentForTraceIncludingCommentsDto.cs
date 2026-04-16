using ChainMates.Server.DTOs.Author;
using ChainMates.Server.DTOs.Comment;

namespace ChainMates.Server.DTOs.Segment
{
    public class SegmentForTraceIncludingCommentsDto
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public AuthorDto Author { get; set; }

        public List<CommentForTraceDto> ChildComments { get; set; } = new List<CommentForTraceDto>();
    }
}
