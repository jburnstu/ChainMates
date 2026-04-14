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
                new SegmentTrace { FinalSegmentId = 2, EarlierSegmentAuthorId = 1 },
                new SegmentTrace { FinalSegmentId = 3, EarlierSegmentAuthorId = 1 }
            };

            var service = new SegmentService(null);

            var result = service.GetJoinableSegmentIdsByAuthor(1, traces);

            Assert.DoesNotContain(2, result);
            Assert.DoesNotContain(3, result);
            Assert.False(true);
        }


    }
}
