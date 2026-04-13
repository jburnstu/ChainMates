using MyApp.Services;
using ReactApp1.Server;
using ReactApp1.Server.Services;


namespace ReactApp1.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void Author_Blocks_All_Descendants()
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
        }


    }
}
