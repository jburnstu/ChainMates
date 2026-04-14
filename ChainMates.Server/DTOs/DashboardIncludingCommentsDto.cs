using ReactApp1.Server.DTOs.Author;
using ReactApp1.Server.DTOs.Segment;

namespace ReactApp1.Server.DTOs
{
    public class DashboardIncludingCommentsDto
    {
        public AuthorDto AuthorInfo { get; set; }
        public List<SegmentHistoryIncludingCommentsDto>  WriteDicts { get; set; }
        public List<SegmentHistoryIncludingCommentsDto> ReviewDicts { get; set; }
        public StartingUrlDto StartingUrlDict { get; set; }
    }
}
