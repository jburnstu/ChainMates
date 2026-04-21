using ChainMates.Server.DTOs.Author;
using ChainMates.Server.DTOs.Comment;

namespace ChainMates.Server.DTOs.Segment
{
    public class HistoricalSegmentDto
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public AuthorDto Author { get; set; }

        public List<HistoricalCommentDto> ChildComments { get; set; } = new List<HistoricalCommentDto>();
    }
}
