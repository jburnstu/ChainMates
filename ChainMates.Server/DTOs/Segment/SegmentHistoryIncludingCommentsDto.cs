using ReactApp1.Server.DTOs.Story;

namespace ReactApp1.Server.DTOs.Segment
{
    public class SegmentHistoryIncludingCommentsDto
    {
        public int Id { get; set; }
        public StoryIncludingCommentsDto StoryData { get; set; }
        public List<SegmentForTraceIncludingCommentsDto> SegmentHistoryList { get; set; }

       

    }
}
