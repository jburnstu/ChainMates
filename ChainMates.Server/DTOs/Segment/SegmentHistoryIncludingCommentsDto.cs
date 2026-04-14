using ChainMates.Server.DTOs.Story;

namespace ChainMates.Server.DTOs.Segment
{
    public class SegmentHistoryIncludingCommentsDto
    {
        public int Id { get; set; }
        public StoryIncludingCommentsDto StoryData { get; set; }
        public List<SegmentForTraceIncludingCommentsDto> SegmentHistoryList { get; set; }

       

    }
}
