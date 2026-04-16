using ChainMates.Server;
using ChainMates.Server.Services;


namespace ChainMates.Tests
{
    public class SegmentServiceTests
    {
        [Fact]
        public void AuthorBlocksAllDescendants()
        {
            var traces = new List<SegmentTrace>
            {
                new SegmentTrace { FinalSegmentId = 2, EarlierSegmentAuthorId = 1 }
            };

            /* For now I've set up segmentService to not take a context when
             * it's not needed -- at some point might split this out into a
             * repository layer or similar. */
            var service = new SegmentService();

            var joinResult = service.GetJoinableSegmentIdsByAuthor(1, traces);
            var moderateResult = service.GetModeratableSegmentIdsByAuthor(1, traces);

            Assert.DoesNotContain(2, joinResult);
            Assert.DoesNotContain(2, moderateResult);
            //Assert.False(true);
        }




    }
}
