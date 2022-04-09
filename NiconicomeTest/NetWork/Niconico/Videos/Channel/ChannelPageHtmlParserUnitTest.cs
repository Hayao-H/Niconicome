using System.Linq;
using Niconicome.Models.Domain.Niconico.Remote.Channel;
using Niconicome.Models.Domain.Utils;
using Niconicome.Models.Playlist;
using NiconicomeTest.Stabs.Models.Domain.Utils;
using NiconicomeTest.Stabs.Models.Playlist;
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
            this.parser = new ChannelPageHtmlParser(new LoggerStub(), new NiconicoUtils(), new VideoInfoContainerStab());
        }

        [Test]
        public void エルフェンリートのHTMLをパースする()
        {
            var info = this.parser!.ParseAndGetIds(Properties.Resources.Elfenlied_Channel);
            IListVideoInfo? ep1 = info.Videos.FirstOrDefault(v => v.NiconicoId.Value == "so32628045");

            Assert.That(info.Videos.Count(), Is.EqualTo(14));
            Assert.That(info.HasNext, Is.False);
            Assert.That(info.ChannelName, Is.EqualTo("エルフェンリート"));
            Assert.That(ep1, Is.Not.Null);
            Assert.That(ep1?.Title.Value, Is.EqualTo("エルフェンリート　第1話 邂逅　"));
        }

        [Test]
        public void Ch7のHTMLをパースする()
        {
            var info = this.parser!.ParseAndGetIds(Properties.Resources.Channel_7);
            Assert.That(info.Videos.Count(), Is.EqualTo(20));
            Assert.That(info.HasNext, Is.True);
            Assert.That(info.NextPageQuery, Is.EqualTo("?&mode=&sort=f&order=d&type=&page=2"));
            Assert.That(info.ChannelName, Is.EqualTo("テレビ東京あにてれちゃんねる"));
        }
    }
}
