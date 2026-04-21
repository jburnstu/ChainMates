using ChainMates.Server.DTOs.Story;

namespace ChainMates.Server.DTOs.Segment
{
    public class SegmentHistoryDto
    {
        public int Id { get; set; }
        public StoryIncludingCommentsDto StoryData { get; set; }
        public List<HistoricalSegmentDto> SegmentHistoryList { get; set; }

       

    }
}
