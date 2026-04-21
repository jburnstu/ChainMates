using ChainMates.Server.DTOs.Story;

namespace ChainMates.Server.DTOs.Segment
{
    public class SegmentHistoryExcludingCommentsDto
    {
        public int Id { get; set; }
        public StoryDto StoryData { get; set; }
        public List<SegmentForTraceExcludingCommentsDto> SegmentHistoryList { get; set; }

    }
}
