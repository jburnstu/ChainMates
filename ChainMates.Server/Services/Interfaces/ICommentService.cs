using ChainMates.Server.DTOs.Comment;


namespace ChainMates.Server.Services
{
    public interface ICommentService
    {

        //Task<int> CreateComment(CommentCreationDto dto, int authorId);
        //Task<CommentPatchDto> UpdateComment(Comment comment, CommentPatchDto dto);

        Task<CommentCreationAndSubmissionDto> CreateAndSubmitComment(CommentCreationAndSubmissionDto dto, int authorId);

        Task<List<HistoricalCommentDto>> GetStoryCommentAndChildrenForHistory(int storyId);

        Task<List<HistoricalCommentDto>> GetHistoricalSegmentCommentAndChildren(int segmentId);

    }
}
