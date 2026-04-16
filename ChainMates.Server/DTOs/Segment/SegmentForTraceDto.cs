using ChainMates.Server.DTOs.Author;

namespace ChainMates.Server.DTOs.Segment
{
    public class SegmentForTraceDto
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public AuthorDto Author { get; set; }
    }
}
