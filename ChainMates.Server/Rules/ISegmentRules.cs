
namespace ChainMates.Server.Rules
{
    public interface ISegmentRules 
    {
        List<int> GetJoinableSegmentIdsByAuthor(int authorId, List<SegmentTrace> traces);

        List<int> GetModeratableSegmentIdsByAuthor(int authorId, List<SegmentTrace> traces);

    }
}
