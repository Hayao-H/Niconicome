using System.Linq;
using Niconicome.Models.Helper.Event.Generic;
using Niconicome.Models.Playlist;
using Niconicome.Models.Playlist.VideoList;
using NiconicomeTest.Stabs.Models.Domain.Local.Store;
using NiconicomeTest.Stabs.Models.Playlist;
using NiconicomeTest.Stabs.Models.Playlist.VideoList;
using NUnit.Framework;

namespace NiconicomeTest.Local.Playlist.Videos
{
    [TestFixture]
    class VideoListContainerUnitTest
    {
        private IVideoListContainer? videoListContainer;

        private ChangeType lastChangeType;

        private string? lastVIdeoNicoID;

        private PlaylistStoreHandlerStab? playlistStoreHandler;

        [SetUp]
        public void SetUp()
        {
            this.playlistStoreHandler = new PlaylistStoreHandlerStab();
            var p = new NonBindableTreePlaylistInfo() { Id=1};
            var cStab = new CurrentStab();
            cStab.SelectedPlaylist.Value = p;
            this.videoListContainer = new VideoListContainer(this.playlistStoreHandler, new VideoHandlerStab(), new VideoListRefresherStab(), cStab);
            this.lastVIdeoNicoID = null;
            this.videoListContainer.ListChanged += (_, e) =>
            {
                this.lastChangeType = e.ChangeType;
                this.lastVIdeoNicoID = e.Data?.NiconicoId.Value;
            };
        }

        [Test]
        public void 動画を追加する()
        {
            var video = new NonBindableListVideoInfo();
            video.NiconicoId.Value = "sm9";
            var result = this.videoListContainer!.Add(video, 1);

            Assert.That(this.lastChangeType, Is.EqualTo(ChangeType.Add));
            Assert.That(this.lastVIdeoNicoID, Is.EqualTo("sm9"));
            Assert.That(this.videoListContainer.Videos.Count, Is.EqualTo(1));
            Assert.That(result.IsSucceeded, Is.True);
            Assert.That(this.playlistStoreHandler!.VideoCount, Is.EqualTo(1));
        }

        [Test]
        public void 重複した動画を追加する()
        {
            var video = new NonBindableListVideoInfo();
            video.NiconicoId.Value = "sm9";
            var video2 = new NonBindableListVideoInfo();
            video2.NiconicoId.Value = "sm9";
            var result = this.videoListContainer!.Add(video, 1);
            var result2 = this.videoListContainer!.Add(video2, 1);

            Assert.That(this.lastChangeType, Is.EqualTo(ChangeType.Add));
            Assert.That(this.lastVIdeoNicoID, Is.EqualTo("sm9"));
            Assert.That(this.videoListContainer.Videos.Count, Is.EqualTo(1));
            Assert.That(result.IsSucceeded, Is.True);
            Assert.That(result2.IsSucceeded, Is.False);
        }

        [Test]
        public void 動画を削除する()
        {
            var video = new NonBindableListVideoInfo();
            video.NiconicoId.Value = "sm9";
            this.videoListContainer!.Add(video);
            var result = this.videoListContainer!.Remove(video, 1);

            Assert.That(this.lastChangeType, Is.EqualTo(ChangeType.Remove));
            Assert.That(this.lastVIdeoNicoID, Is.EqualTo("sm9"));
            Assert.That(this.videoListContainer.Videos.Count, Is.EqualTo(0));
            Assert.That(result.IsSucceeded, Is.True);
            Assert.That(this.playlistStoreHandler!.VideoCount, Is.EqualTo(0));
        }

        [Test]
        public void 存在しない動画を削除する()
        {
            var video = new NonBindableListVideoInfo() ;
            video.NiconicoId.Value = "sm9";
            var result = this.videoListContainer!.Remove(video);

            Assert.That(this.lastVIdeoNicoID, Is.Null);
            Assert.That(this.videoListContainer.Videos.Count, Is.EqualTo(0));
            Assert.That(result.IsSucceeded, Is.False);
        }

    }
}
