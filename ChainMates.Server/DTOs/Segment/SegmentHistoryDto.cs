using ChainMates.Server.DTOs.Story;

namespace ChainMates.Server.DTOs.Segment
{
    public class SegmentHistoryDto
    {
        public int Id { get; set; }
        public StoryDto StoryData { get; set; }
        public List<SegmentForTraceDto> SegmentHistoryList { get; set; }

    }
}
