using ChainMates.Server.DTOs.Author;
using ChainMates.Server.DTOs.Segment;

namespace ChainMates.Server.DTOs
{
    public class DashboardExcludingRelationsDto
    {
        public AuthorDto AuthorInfo { get; set; }
        public List<SegmentHistoryIncludingCommentsDto>  WriteDicts { get; set; }
        public List<SegmentHistoryIncludingCommentsDto> ReviewDicts { get; set; }
        public StartingUrlDto StartingUrlDict { get; set; }
    }
}
