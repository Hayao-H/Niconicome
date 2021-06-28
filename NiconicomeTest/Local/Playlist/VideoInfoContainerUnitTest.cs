using Niconicome.Models.Playlist;
using NUnit.Framework;

namespace NiconicomeTest.Local.Playlist
{
    [TestFixture]
    class VideoInfoContainerUnitTest
    {
        private IVideoInfoContainer? container;

        [SetUp]
        public void SetUp()
        {
            this.container = new VideoInfoContainer();
        }

        [Test]
        public void 動画を取得する()
        {
            IListVideoInfo? onmyoji = this.container!.GetVideo("sm9");

            Assert.That(onmyoji, Is.Not.Null);
            Assert.That(this.container!.Count, Is.EqualTo(1));
        }

        [Test]
        public void インスタンスの同一性を確認する()
        {
            string title = "レッツゴー！陰陽師";
            IListVideoInfo onmyoji = this.container!.GetVideo("sm9");
            onmyoji.Title.Value = title;

            IListVideoInfo reOnmyoji = this.container!.GetVideo("sm9");
            Assert.That(reOnmyoji.Title.Value, Is.EqualTo(title));

        }

        [Test]
        public void クリア()
        {
            this.container!.GetVideo("1");
            this.container!.GetVideo("2");
            this.container!.GetVideo("3");
            this.container!.GetVideo("4");
            this.container!.GetVideo("5");

            Assert.That(this.container!.Count, Is.EqualTo(5));

            this.container.Clear();

            Assert.That(this.container!.Count, Is.EqualTo(0));

        }
    }
}
