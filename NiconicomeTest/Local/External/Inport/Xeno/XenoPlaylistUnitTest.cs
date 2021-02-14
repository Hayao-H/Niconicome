using NUnit.Framework;
using Niconicome.Models.Domain.Local.External.Import.Xeno;

namespace NiconicomeTest.Local.External.Inport.Xeno
{
    [TestFixture]
    class XenoPlaylistUnitTest
    {
        IXenoPlaylist? grandChild;

        IXenoPlaylist? child;

        IXenoPlaylist? parent;

        [SetUp]
        public void SetUp()
        {
            this.parent = new XenoPlaylist("ルート");
            this.child = new XenoPlaylist("子", this.parent);
            this.grandChild = new XenoPlaylist("孫","channel", this.child);
        }

        [Test]
        public void レイヤーを調査()
        {
            Assert.That(this.parent!.Layer, Is.EqualTo(1));
            Assert.That(this.child!.Layer, Is.EqualTo(2));
            Assert.That(this.grandChild!.Layer, Is.EqualTo(3));
        }

        [Test]
        public void 子プレイリストを調査()
        {
            Assert.That(this.parent!.ChildPlaylists.Contains(this.child!), Is.True);
            Assert.That(this.child!.ChildPlaylists.Contains(this.grandChild!), Is.True);
        }

        [Test]
        public void ルートフラグを調査()
        {
            Assert.That(this.parent!.IsRootPlaylist, Is.True);
            Assert.That(this.child!.IsRootPlaylist, Is.False);
            Assert.That(this.grandChild!.IsRootPlaylist, Is.False);
        }

        [Test]
        public void チャンネルフラグ()
        {
            Assert.That(this.parent!.IsChannel, Is.False);
            Assert.That(this.grandChild!.IsChannel, Is.True);
        }
    }
}
