
using ChainMates.Server.DTOs.Segment;

namespace ChainMates.Server.Services
{
    public interface ISegmentService
    {
        Task<Segment> CreateSegment(SegmentCreationDto dto, int authorId, bool save);
        Task<SegmentHistoryDto> GetSegmentHistoryBySegment(int segmentId);
    }
}
