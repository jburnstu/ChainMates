
using ChainMates.Server.DTOs.Segment;

namespace ChainMates.Server.Services
{
    public interface IStoryService
    {
        Task<Story> GetStoryBySegment(int segmentId);
    }
}
