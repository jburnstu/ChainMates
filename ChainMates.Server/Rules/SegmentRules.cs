

namespace ChainMates.Server.Rules
{
    public class SegmentRules : ISegmentRules
    {

        public SegmentRules() { }

        public List<int> GetJoinableSegmentIdsByAuthor(int authorId, List<SegmentTrace> traces)
        {
            // Blocked if any previous segment was written by the author
            // Might also introduce "blocked if any future segment written by the author"
            var blockedSegmentIdList = traces
                .Where(t => t.EarlierSegmentAuthorId == authorId)
                .Select(t => t.FinalSegmentId)
                .ToHashSet();

            return traces
                .Where(t => t.FinalSegmentStatusId == (int)Enums.SegmentStatusEnum.AvailableForAddition)
                .Select(t => t.FinalSegmentId)
                .Distinct()
                .Where(id => !blockedSegmentIdList.Contains(id))
                .ToList();

        }

        public List<int> GetModeratableSegmentIdsByAuthor(int authorId, List<SegmentTrace> traces)
        {
            // Blocked if any previous segment was written by the author
            var blockedSegmentIdList = traces
                .Where(t => t.EarlierSegmentAuthorId == authorId)
                .Select(t => t.FinalSegmentId)
                .ToHashSet();

            return traces
                .Where(t => t.FinalSegmentStatusId == (int)Enums.SegmentStatusEnum.AvailableForModeration)
                .Select(t => t.FinalSegmentId)
                .Distinct()
                .Where(id => !blockedSegmentIdList.Contains(id))
                .ToList();

        }

    }
}
