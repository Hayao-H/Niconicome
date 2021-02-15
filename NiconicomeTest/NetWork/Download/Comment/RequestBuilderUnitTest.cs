using NUnit.Framework;
using Niconicome.Models.Domain.Niconico.Download.Comment;
using Niconicome.Models.Domain.Niconico.Watch;
using System.Net.Http;
using NiconicomeTest.Stabs.Models.Domain.Niconico.NicoHttpStabs;
using System.Threading.Tasks;
using System;

namespace NiconicomeTest.NetWork.Download.Comment
{
    [TestFixture]
    class RequestBuilderUnitTest
    {
        private readonly IDmcInfo? dmcInfo;

        private ICommentRequestBuilder? commentRequestBuilder;

        public RequestBuilderUnitTest()
        {
            var parser = new WatchPageHtmlParser();
            this.dmcInfo = parser.GetDmcInfo(Properties.Resources.Niconico_Onmyoji,"sm9", WatchInfoOptions.Default);
        }

        [SetUp]
        public void SetUp()
        {
            var content = new StringContent("threadkey=1611461521.GIWx_XyM-juZoqSIdvEOU9rIYy8&force_184=1");
            var http = new NicoHttpStab(content, content);
            var auth = new OfficialVideoUtils(http);
            this.commentRequestBuilder = new CommentRequestBuilder(auth);
        }

        [Test]
        public async Task リクエストを構築する()
        {
            string data = await this.commentRequestBuilder!.GetRequestDataAsync(this.dmcInfo!, new CommentOptions());
            Assert.That(data.Length, Is.GreaterThan(0));
        }

    }
}
