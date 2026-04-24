using ChainMates.Server.DTOs.Author;

namespace ChainMates.Server.DTOs.Story
{
    public class StoryInfoIncludingStructureDto
    {
        public int Id { get; set; }
        public AuthorDto Author {  get; set; }
        public string? Title { get; set; }
        public Dictionary<int,List<int>> Structure { get; set; }

    }
}
