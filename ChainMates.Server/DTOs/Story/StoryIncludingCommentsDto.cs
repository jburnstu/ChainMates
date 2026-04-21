using ChainMates.Server.DTOs.Comment;

namespace ChainMates.Server.DTOs.Story
{
    public class StoryIncludingCommentsDto
    {
        //public int AuthorId {  get; set; }
        public string? Title { get; set; }
        public int? MaxSegments { get; set; }
        public int? MaxSegmentLength { get; set; }
        public int? MinSegmentLength { get; set; }
        public int? MaxBranches { get; set; }
        public bool? IsItMature { get; set; }

        public List<HistoricalCommentDto> ChildComments { get; set; }  = new List<HistoricalCommentDto>();
    }
}
