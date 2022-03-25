using NUnit.Framework;
using Niconicome.Models.Domain.Local.External.Import.Xeno;
using NiconicomeTest.Stabs.Models.Domain.Utils;
using System.Linq;
using NiconicomeTest.Stabs.Models.Domain.Local.Extarnal.Inport.Xeno;

namespace NiconicomeTest.Local.External.Inport.Xeno
{
    [TestFixture]
    class XenoVideoNodeParserUnitTest
    {
        private IXenoVideoNodeParser? parser;

        [SetUp]
        public void SetUp()
        {
            this.parser = new XenoVideoNodeParser(new LoggerStub());
        }

        [TestCase]
        public void Xenoの動画情報ファイルを解析する()
        {
            var content = Properties.Resources.Xeno_Video_File;
            var result = this.parser!.Parse(content);
            Assert.That(result.SucceededCount, Is.EqualTo(133));
            Assert.That(result.Videos.First().NiconicoId, Is.EqualTo("sm30703222"));
        }
    }

    [TestFixture]
    class XenoPlaylistParserUnitTest
    {
        private IXenoRootParser? parser;

        [SetUp]
        public void SetUp()
        {
            this.parser = new XenoRootParser(new LoggerStub(),new XenoVideoNodeParserStab());
        }

        [Test]
        public void Xenoのプレイリスト情報ファイルを解析する()
        {
            var content = Properties.Resources.Xeno_Playlist_File;
            var result = this.parser!.ParseText(content);

            Assert.That(result.SucceededCount,Is.EqualTo(62));
            Assert.That(result.Playlists.First().Name, Is.EqualTo("履歴"));
            Assert.That(result.Playlists[7].ParentPlaylist?.Name, Is.EqualTo("新しいフォルダ"));
        }
    }
}
