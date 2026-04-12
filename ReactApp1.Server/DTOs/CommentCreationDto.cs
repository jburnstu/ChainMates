namespace ReactApp1.Server.DTOs
{
    public class CommentCreationDto
    {
        public required  int CommentTypeId { get; set; }
        public required int ParentId { get; set; }
    }
}
