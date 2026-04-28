using ChainMates.Server;
using ChainMates.Server.Rules;
using ChainMates.Server.Services;


namespace ChainMates.Tests
{
    public class SegmentServiceTests
    {
        [Fact]
        public async Task AuthorBlocksAllDescendants()
        {
            var traces = new List<SegmentTrace>
            {
                new SegmentTrace { FinalSegmentId = 2, EarlierSegmentAuthorId = 1 }
            };

            var rules = new SegmentRules();

            var joinResult = rules.GetJoinableSegmentIdsByAuthor(1, traces);
            var moderateResult = rules.GetModeratableSegmentIdsByAuthor(1, traces);

            Assert.DoesNotContain(2, joinResult);
            Assert.DoesNotContain(2, moderateResult);

        }




    }
}
