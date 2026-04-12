namespace ReactApp1.Server.DTOs.Segment
{
    public class SegmentCreationDto
    {
        //public int AuthorId { get; set; }
        public required int StoryId { get; set; }
        public int? SegmentStatusId { get; set; }
        public int? PreviousSegmentId { get; set; }
        public string? Content { get; set; }
    }
}
