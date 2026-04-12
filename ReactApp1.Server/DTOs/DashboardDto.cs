namespace ReactApp1.Server.DTOs
{
    public class DashboardDto
    {
        public AuthorDto AuthorInfo { get; set; }
        public List<SegmentHistoryDto>  WriteDicts { get; set; }
        public List<SegmentHistoryDto> ReviewDicts { get; set; }
        public StartingUrlDto StartingUrlDict { get; set; }
    }
}
