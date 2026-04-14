using ReactApp1.Server.DTOs.Author;

namespace ReactApp1.Server.DTOs.Segment
{
    public class SegmentForTraceDto
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public AuthorDto Author { get; set; }
    }
}
