namespace ReactApp1.Server.DTOs
{
    public class RandomCreationDto
    {
        public int NumberOfAuthors { get; set; }

        public int NumberOfSegments { get; set; }
        public int SegmentCharLength { get; set; } = 200;
    }
}
