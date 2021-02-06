using System.Linq;
using Niconicome.Models.Domain.Niconico.Video.Channel;
using Niconicome.Models.Domain.Utils;
using NiconicomeTest.Stabs.Models.Domain.Utils;
using NUnit.Framework;

namespace NiconicomeTest.NetWork.Videos.Channel
{
    [TestFixture]
    class ChannelPageHtmlParserUnitTest
    {
        private IChannelPageHtmlParser? parser;

        [SetUp]
        public void SetUp()
        {
            this.parser = new ChannelPageHtmlParser(new LoggerStab(), new NiconicoUtils());
        }

        [Test]
        public void HTMLをパースする()
        {
            var ids = this.parser!.ParseAndGetIds(Properties.Resources.Elfenlied_Channel);
            Assert.That(ids.Count(), Is.EqualTo(14));
        }
    }
}
