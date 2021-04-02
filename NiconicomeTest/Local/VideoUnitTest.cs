using NUnit.Framework;
using Niconicome.Models.Domain.Local;
using System.Linq;
using System;
using System.Collections.Generic;
using STypes = Niconicome.Models.Domain.Local.Store.Types;
using Niconicome.Models.Playlist;
using Niconicome.Models.Domain.Local.Store;

namespace NiconicomeTest.Local.Video
{
    [TestFixture]
    class VideoUnitTest
    {
        private IDataBase? databaseInstance;

        private IVideoStoreHandler? handler;

        private IPlaylistStoreHandler? playlistHandler;

        private int playlist1Id;

        private int playlist2Id;

        [SetUp]
        public void SetUp()
        {
            this.databaseInstance = Static.DataBaseInstance;

            //リセットする
            this.databaseInstance.Clear(STypes::Video.TableName);

            //動画を操作する為のハンドラを作成する
            this.handler = new VideoStoreHandler(this.databaseInstance);

            //プレイリストを操作する為のハンドラを作成する
            this.playlistHandler = new PlaylistStoreHandler(this.databaseInstance, this.handler);

            ///ルート直下にテスト用のプレイリストを作成する
            int rootId = this.playlistHandler.GetRootPlaylist().Id;
            this.playlist1Id = this.playlistHandler.AddPlaylist(rootId, "テストプレイリスト1");
            this.playlist2Id = this.playlistHandler.AddPlaylist(rootId, "テストプレイリスト2");

        }

        [Test]
        public void 動画を追加する()
        {
            var videoInfo = new NonBindableListVideoInfo()
            {
                NiconicoId = "sm9",
                Title = "テスト動画"
            };

            int videoId = this.handler?.AddVideo(videoInfo, this.playlist1Id) ?? -1;

            Assert.Greater(videoId, -1);
            Assert.IsTrue(this.handler?.Exists(videoId));
            Assert.IsTrue(this.handler?.Exists("sm9"));
        }

        [Test]
        public void 複数プレイリストに単一動画を追加する()
        {
            var videoInfo = new NonBindableListVideoInfo()
            {
                NiconicoId = "sm9",
                Title = "テスト動画"
            };

            int videoId1 = this.handler?.AddVideo(videoInfo, this.playlist1Id) ?? -1;
            int videoId2 = this.handler?.AddVideo(videoInfo, this.playlist2Id) ?? -1;

            var video = this.handler?.GetVideo(videoId1);

            Assert.Greater(videoId1, -1);
            Assert.Greater(videoId2, -1);
            Assert.AreEqual(videoId1, videoId2);
            Assert.IsNotNull(video);
            Assert.IsNotNull(video?.PlaylistIds);
            Assert.AreEqual(2, video?.PlaylistIds?.Count);
        }

        [Test]
        public void 動画を削除する()
        {
            var videoInfo = new NonBindableListVideoInfo()
            {
                NiconicoId = "sm9",
                Title = "テスト動画"
            };

            int videoId = this.handler?.AddVideo(videoInfo, this.playlist1Id) ?? -1;
            this.handler?.RemoveVideo(videoId, this.playlist1Id);

            Assert.Greater(videoId, -1);
            Assert.IsFalse(this.handler?.Exists(videoId));
            Assert.IsFalse(this.handler?.Exists("sm9"));
        }

        [Test]
        public void 複数プレイリストに追加した動画を削除する()
        {
            var videoInfo = new NonBindableListVideoInfo()
            {
                NiconicoId = "sm9",
                Title = "テスト動画"
            };

            int videoId = this.handler?.AddVideo(videoInfo, this.playlist1Id) ?? -1;
            this.handler?.AddVideo(videoInfo, this.playlist2Id);
            this.handler?.RemoveVideo(videoId, this.playlist1Id);

            Assert.Greater(videoId, -1);
            Assert.IsTrue(this.handler?.Exists(videoId));
            Assert.IsTrue(this.handler?.Exists("sm9"));
        }
    }
}
