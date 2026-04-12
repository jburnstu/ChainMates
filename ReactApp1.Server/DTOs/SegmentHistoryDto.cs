namespace ReactApp1.Server.DTOs
{
    public class SegmentHistoryDto
    {
        public int Id { get; set; }
        public StoryDto StoryData { get; set; }
        public List<SegmentForTraceDto> SegmentHistoryList { get; set; }

    }
}
