namespace ChainMates.Server.DTOs.Segment
{
    public class InitialSegmentCreationDto
    {
        //public int AuthorId { get; set; }
        public int? StoryId { get; set; }
        public int? SegmentStatusId { get; set; }
        public string? Content { get; set; }
    }
}
