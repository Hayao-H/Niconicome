using NUnit.Framework;
using Niconicome.Models.Domain.Local.External.Import.Xeno;
using NiconicomeTest.Stabs.Models.Domain.Utils;
using NiconicomeTest.Stabs.Models.Domain.Local.Extarnal.Inport.Xeno;

namespace NiconicomeTest.Local.External.Inport.Xeno
{
    [TestFixture]
    class XenoConverterUnitTest
    {
        private IXenoPlaylistConverter? converter;

        private IXenoPlaylist? playlist;

        [SetUp]
        public void Setup()
        {
            this.converter = new XenoPlaylistConverter();
            this.playlist = new XenoRootParser(new LoggerStab(), new XenoVideoNodeParserStab()).ParseText(Properties.Resources.Xeno_Playlist_File).RootPlaylist!;
        }

        [Test]
        public void TreePlaylistInfoに変換する()
        {
            var converted = this.converter!.ConvertToTreePlaylistInfo(this.playlist!);
            Assert.That(converted.Name.Value, Is.EqualTo("Root"));
            Assert.That(converted.Children.Count, Is.EqualTo(3));
        }
    }
}
