using ChainMates.Server.DTOs.Author;

namespace ChainMates.Server.DTOs
{
    public class RelationInfoDto
    {
        public List<AuthorDto> AuthorsWhoYouFollow { get; set; }
        public List<AuthorDto> AuthorsWhoFollowYou { get; set; }
        public List<CircleDto> Circles { get; set; }
    }
}
