using ChainMates.Server.DTOs.Author;

namespace ChainMates.Server.DTOs
{
    public class RelationInfoDto
    {
        public List<AuthorDto> FollowingAuthors { get; set; }
        public List<AuthorDto> FollowedAuthors { get; set; }
        public List<CircleDto> Circles { get; set; }
    }
}
