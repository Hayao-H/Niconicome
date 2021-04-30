using System.Linq;
using Niconicome.Models.Domain.Niconico.Video.Channel;
using Niconicome.Models.Domain.Utils;
using NiconicomeTest.Stabs.Models.Domain.Utils;
using NUnit.Framework;

namespace NiconicomeTest.NetWork.Niconico.Videos.Channel
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
        public void エルフェンリートのHTMLをパースする()
        {
            var info = this.parser!.ParseAndGetIds(Properties.Resources.Elfenlied_Channel);
            Assert.That(info.IDs.Count(), Is.EqualTo(14));
            Assert.That(info.HasNext, Is.False);
            Assert.That(info.ChannelName, Is.EqualTo("エルフェンリート"));
        }

        [Test]
        public void Ch7のHTMLをパースする()
        {
            var info = this.parser!.ParseAndGetIds(Properties.Resources.Channel_7);
            Assert.That(info.IDs.Count(), Is.EqualTo(20));
            Assert.That(info.HasNext, Is.True);
            Assert.That(info.NextPageQuery, Is.EqualTo("?&mode=&sort=f&order=d&type=&page=2"));
            Assert.That(info.ChannelName, Is.EqualTo("テレビ東京あにてれちゃんねる"));
        }
    }
}
