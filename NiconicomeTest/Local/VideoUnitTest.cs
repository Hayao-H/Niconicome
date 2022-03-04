using Niconicome.Models.Domain.Local;
using Niconicome.Models.Domain.Local.Store;
using Niconicome.Models.Helper.Result;
using NiconicomeTest.Stabs.Models.Domain.Utils;
using NUnit.Framework;
using STypes = Niconicome.Models.Domain.Local.Store.Types;

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
            this.handler = new VideoStoreHandler(this.databaseInstance, new LoggerStab());

            //プレイリストを操作する為のハンドラを作成する
            this.playlistHandler = new PlaylistStoreHandler(this.databaseInstance, new LoggerStab());
            this.playlistHandler.Initialize();

            ///ルート直下にテスト用のプレイリストを作成する
            ///
            int rootId = this.playlistHandler.GetRootPlaylist().Data!.Id;
            this.playlist1Id = this.playlistHandler.AddPlaylist(rootId, "テストプレイリスト1").Data;
            this.playlist2Id = this.playlistHandler.AddPlaylist(rootId, "テストプレイリスト2").Data;

        }

        [Test]
        public void 動画を追加する()
        {
            var videoInfo = new STypes::Video();
            videoInfo.NiconicoId = "sm9";
            videoInfo.Title = "テスト動画";

            IAttemptResult<int> videoResult = this.handler!.AddVideo(videoInfo);

            int videoId = videoResult.Data;

            Assert.That(videoResult.IsSucceeded, Is.True);
            Assert.That(this.handler.Exists(videoId), Is.True);
            Assert.That(this.handler.Exists("sm9"), Is.True);
        }

        [Test]
        public void 複数プレイリストに単一動画を追加する()
        {
            var videoInfo = new STypes::Video();
            videoInfo.NiconicoId = "sm9";
            videoInfo.Title = "テスト動画";

            IAttemptResult<int> videoResult = this.handler!.AddVideo(videoInfo);

            IAttemptResult<int> v1Result = this.handler!.AddVideo(videoInfo);
            IAttemptResult<int> v2Result = this.handler.AddVideo(videoInfo);

            int videoId1 = v1Result.Data;
            int videoId2 = v2Result.Data;

            IAttemptResult<STypes::Video> video = this.handler.GetVideo(videoId1);

            Assert.That(v1Result.IsSucceeded, Is.True);
            Assert.That(v2Result.IsSucceeded, Is.True);
            Assert.AreEqual(videoId1, videoId2);
            Assert.That(video.IsSucceeded, Is.True);
            Assert.That(video.Data, Is.Not.Null);
            Assert.That(video.Data!.Id, Is.EqualTo(videoId1));
            Assert.That(video.Data.NiconicoId, Is.EqualTo("sm9"));
            Assert.That(video.Data.Title, Is.EqualTo("テスト動画"));
        }

        [Test]
        public void 動画を削除する()
        {
            var videoInfo = new STypes::Video();
            videoInfo.NiconicoId = "sm9";
            videoInfo.Title = "テスト動画";

            IAttemptResult<int> videoResult = this.handler!.AddVideo(videoInfo);

            int videoId = videoResult.Data;

            IAttemptResult result = this.handler.RemoveVideo(videoId);

            Assert.That(result.IsSucceeded, Is.True);
            Assert.That(this.handler.Exists(videoId), Is.False);
            Assert.That(this.handler.Exists("sm9"), Is.False);
        }

    }
}
