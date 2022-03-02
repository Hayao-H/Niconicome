using System;
using System.Linq;
using System.Reactive.Linq;
using Niconicome.Models.Domain.Local.Store.Types;
using Niconicome.Models.Helper.Event.Generic;
using Niconicome.Models.Helper.Result;
using Niconicome.Models.Playlist;
using Niconicome.Models.Playlist.Playlist;
using Niconicome.Models.Playlist.VideoList;
using NiconicomeTest.Stabs.Models.Domain.Local.Store;
using NiconicomeTest.Stabs.Models.Domain.Utils;
using NiconicomeTest.Stabs.Models.Playlist;
using NiconicomeTest.Stabs.Models.Playlist.Playlist;
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

        private ICurrent? cStab;

        [SetUp]
        public void SetUp()
        {
            this.playlistStoreHandler = new PlaylistStoreHandlerStab();
            ITreePlaylistInfo? p = new NonBindableTreePlaylistInfo() { Id = 1 };
            this.cStab = new CurrentStab();
            cStab.SelectedPlaylist.Value = p;
            this.videoListContainer = new VideoListContainer(new PlaylistHandlerStab(), new VideoHandlerStab(), new VideoListRefresherStab(), this.cStab, new LoggerStab());
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
            video.IsSelected.Value = true;
            var result = this.videoListContainer!.Add(video, 1);

            Assert.That(this.lastChangeType, Is.EqualTo(ChangeType.Add));
            Assert.That(this.lastVIdeoNicoID, Is.EqualTo("sm9"));
            Assert.That(this.videoListContainer.Videos.Count, Is.EqualTo(1));
            Assert.That(result.IsSucceeded, Is.True);
            Assert.That(this.cStab!.SelectedVideos.Value, Is.EqualTo(1));
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
            video.IsSelected.Value = true;
            this.videoListContainer!.Add(video);
            var result = this.videoListContainer!.Remove(video, 1);

            Assert.That(result.IsSucceeded, Is.True);
            Assert.That(this.lastChangeType, Is.EqualTo(ChangeType.Remove));
            Assert.That(this.lastVIdeoNicoID, Is.EqualTo("sm9"));
            Assert.That(this.videoListContainer.Videos.Count, Is.EqualTo(0));
            Assert.That(result.IsSucceeded, Is.True);
            Assert.That(this.cStab!.SelectedVideos.Value, Is.Zero);
        }

        [Test]
        public void 存在しない動画を削除する()
        {
            var video = new NonBindableListVideoInfo();
            video.NiconicoId.Value = "sm9";
            var result = this.videoListContainer!.Remove(video);

            Assert.That(this.lastVIdeoNicoID, Is.Null);
            Assert.That(this.videoListContainer.Videos.Count, Is.EqualTo(0));
            Assert.That(result.IsSucceeded, Is.False);
        }

        [Test]
        public void すべての動画にチェックを入れる()
        {
            void check(bool value)
            {
                if (value)
                {
                    this.cStab!.SelectedVideos.Value++;
                }
            }

            var video1 = new NonBindableListVideoInfo() { NiconicoId = new Reactive.Bindings.ReactiveProperty<string>("1") };
            var video2 = new NonBindableListVideoInfo() { NiconicoId = new Reactive.Bindings.ReactiveProperty<string>("2") };
            var video3 = new NonBindableListVideoInfo() { NiconicoId = new Reactive.Bindings.ReactiveProperty<string>("3") };
            video1.IsSelected.Subscribe(value => check(value));
            video2.IsSelected.Subscribe(value => check(value));
            video3.IsSelected.Subscribe(value => check(value));

            this.videoListContainer!.AddRange(new IListVideoInfo[] { video1, video2, video3 });
            this.videoListContainer!.ForEach(v => v.IsSelected.Value = true);


            foreach (var video in this.videoListContainer!.Videos)
            {
                Assert.That(video.IsSelected.Value, Is.True);
            }

            Assert.That(this.cStab!.SelectedVideos.Value, Is.EqualTo(3));


        }

        ///[TestCase(VideoSortType.Register, 1)]
        ///[TestCase(VideoSortType.Title, 3)]
        ///[TestCase(VideoSortType.NiconicoID, 3)]
        public void 動画を並び替える(VideoSortType sortType, int expectedID)
        {
            var video1 = new NonBindableListVideoInfo() { Id = new Reactive.Bindings.ReactiveProperty<int>(1), Title = new Reactive.Bindings.ReactiveProperty<string>("3"), NiconicoId = new Reactive.Bindings.ReactiveProperty<string>("3") };
            var video2 = new NonBindableListVideoInfo() { Id = new Reactive.Bindings.ReactiveProperty<int>(2), Title = new Reactive.Bindings.ReactiveProperty<string>("2"), NiconicoId = new Reactive.Bindings.ReactiveProperty<string>("2") };
            var video3 = new NonBindableListVideoInfo() { Id = new Reactive.Bindings.ReactiveProperty<int>(3), Title = new Reactive.Bindings.ReactiveProperty<string>("1"), NiconicoId = new Reactive.Bindings.ReactiveProperty<string>("1") };
            this.videoListContainer!.AddRange(new IListVideoInfo[] { video1, video2, video3 });
            this.videoListContainer!.Sort(sortType, false);

            Assert.That(this.videoListContainer!.Videos.First().Id.Value, Is.EqualTo(expectedID));
        }

        ///[TestCase(0, false, "1")]
        ///[TestCase(1, true, "2")]
        ///[TestCase(2, true, "1")]
        public void 動画をひとつ前に挿入する(int index, bool expectedResult, int initialID)
        {
            var video1 = new NonBindableListVideoInfo() { Id = new Reactive.Bindings.ReactiveProperty<int>(1), Title = new Reactive.Bindings.ReactiveProperty<string>("3"), NiconicoId = new Reactive.Bindings.ReactiveProperty<string>("1") };
            var video2 = new NonBindableListVideoInfo() { Id = new Reactive.Bindings.ReactiveProperty<int>(2), Title = new Reactive.Bindings.ReactiveProperty<string>("2"), NiconicoId = new Reactive.Bindings.ReactiveProperty<string>("2") };
            var video3 = new NonBindableListVideoInfo() { Id = new Reactive.Bindings.ReactiveProperty<int>(3), Title = new Reactive.Bindings.ReactiveProperty<string>("1"), NiconicoId = new Reactive.Bindings.ReactiveProperty<string>("3") };
            this.videoListContainer!.AddRange(new IListVideoInfo[] { video1, video2, video3 });

            var result = this.videoListContainer.MovevideotoPrev(index);

            Assert.That(result.IsSucceeded, Is.EqualTo(expectedResult));
            Assert.That(this.videoListContainer.Videos[0].Id.Value, Is.EqualTo(initialID));
        }

        ///[TestCase(0, true, "3")]
        ///[TestCase(1, true, "2")]
        ///[TestCase(2, false, "3")]
        public void 動画をひとつ後ろに挿入する(int index, bool expectedResult, int lastID)
        {
            var video1 = new NonBindableListVideoInfo() { Id = new Reactive.Bindings.ReactiveProperty<int>(1), Title = new Reactive.Bindings.ReactiveProperty<string>("3"), NiconicoId = new Reactive.Bindings.ReactiveProperty<string>("1") };
            var video2 = new NonBindableListVideoInfo() { Id = new Reactive.Bindings.ReactiveProperty<int>(2), Title = new Reactive.Bindings.ReactiveProperty<string>("2"), NiconicoId = new Reactive.Bindings.ReactiveProperty<string>("2") };
            var video3 = new NonBindableListVideoInfo() { Id = new Reactive.Bindings.ReactiveProperty<int>(3), Title = new Reactive.Bindings.ReactiveProperty<string>("1"), NiconicoId = new Reactive.Bindings.ReactiveProperty<string>("3") };
            this.videoListContainer!.AddRange(new IListVideoInfo[] { video1, video2, video3 });

            var result = this.videoListContainer.MovevideotoForward(index);

            Assert.That(result.IsSucceeded, Is.EqualTo(expectedResult));
            Assert.That(this.videoListContainer.Videos[2].Id.Value, Is.EqualTo(lastID));

        }

        [Test]
        public void 動画を更新する()
        {
            var count = 100;
            var original = new NonBindableListVideoInfo() { NiconicoId = new Reactive.Bindings.ReactiveProperty<string>("1") };
            var newItem = new NonBindableListVideoInfo() { NiconicoId = new Reactive.Bindings.ReactiveProperty<string>("1"), ViewCount = new Reactive.Bindings.ReactiveProperty<int>(count) };

            this.videoListContainer!.Add(original);
            IAttemptResult result= this.videoListContainer.Update(newItem);

            IListVideoInfo video = this.videoListContainer.Videos.First();

            Assert.That(result.IsSucceeded, Is.True);
            Assert.That(video.ViewCount.Value, Is.EqualTo(count));
        }

    }
}
