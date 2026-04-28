
using ChainMates.Server.DTOs.Author;
using ChainMates.Server.DTOs.Segment;

namespace ChainMates.Server.Services
{
    public interface ISegmentService
    {

        Task<Segment> GetSegment(int id);
        Task<Segment> CreateSegment(SegmentCreationDto dto, int authorId, bool save);
        Task<SegmentHistoryDto> GetSegmentHistoryBySegment(int segmentId);
        Task<List<int>> GetSegmentIdsByAuthorIdAndStatusId(int authorId, int segmentStatusId);

        Task<List<int>> GetModeratedSegmentIdsByAuthorId(int authorId);

        Task<string> UpdateSegmentContent(int segmentId, string content);

        Task<string> SubmitSegmentForModeration(int segmentId, string content);

        Task<ModerationAssignment> CreateModerationAssignment(int segmentId, int authorId);


        Task<int> ApproveModeration(int segmentId, int authorId);

        Task<string> AbandonSegment(int segmentId, string content);

        Task<List<int>> GetJoinableSegmentIdsByAuthor(int authorId);

        Task<List<int>> GetModeratableSegmentIdsByAuthor(int authorId);
    }
}
