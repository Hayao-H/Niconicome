using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Playlist;
using Niconicome.Models.Playlist.Playlist;
using STypes = Niconicome.Models.Domain.Local.Store.Types;
using NiconicomeTest.Local.Playlist.Playlist;

namespace NiconicomeTest.Local.Playlist
{
    [TestFixture]
    class TreePlaylistInfoUnitTest
    {
        [Test]
        public void データベース型からTreePlaylistInfo型への変換()
        {
            var storeData = new STypes::Playlist()
            {
                Id = 1,
                ParentPlaylist = new STypes.Playlist() { Id = 1 },
                IsRoot = true,
                PlaylistName = "テスト"
            };
            var playlistInfo = NonBindableTreePlaylistInfo.ConvertToTreePlaylistInfo(storeData);

            //プレイリスト名
            Assert.AreEqual("テスト", storeData.PlaylistName);
            //ID
            Assert.AreEqual(playlistInfo.Id, 1);
            //親のID
            Assert.AreEqual(playlistInfo.ParentId, 1);
            //ルートフラグ
            Assert.IsTrue(playlistInfo.IsRoot);
        }
    }

    [TestFixture]
    class PlaylistInfoHandlerUnitTest
    {
        private IPlaylistTreeHandler handler = new PlaylistTreeHandler(new PlaylistSettingsHandlerStab());

        [SetUp]
        public void SetUp()
        {
            ///・名前
            ///各IDを漢数字に置き換えたもの
            ///・ツリー構造
            ///親(1)─子(2)─子(4)─子(5)
            ///     └子(3)       ┠子(6)
            ///                   └子(7)
            var first = new STypes::Playlist() { Id = 1, IsRoot = true, PlaylistName = "一", IsTemporary = true };
            var second = new STypes::Playlist() { Id = 2, PlaylistName = "二", IsDownloadFailedHistory = true };
            var third = new STypes::Playlist() { Id = 3, PlaylistName = "三", IsDownloadSucceededHistory = true };
            var fourth = new STypes::Playlist() { Id = 4, PlaylistName = "四" };
            var fifth = new STypes::Playlist() { Id = 5, PlaylistName = "五" };
            var sixth = new STypes::Playlist() { Id = 6, PlaylistName = "六" };
            var seventh = new STypes::Playlist() { Id = 7, PlaylistName = "七" };

            //親を設定
            second.ParentPlaylist = first;
            third.ParentPlaylist = first;
            fourth.ParentPlaylist = second;
            fifth.ParentPlaylist = fourth;
            sixth.ParentPlaylist = fourth;
            seventh.ParentPlaylist = fourth;

            //全て追加
            var playlists = new List<STypes::Playlist>() { first, second, third, fourth, fifth, sixth, seventh };
            this.handler = new PlaylistTreeHandler(new PlaylistSettingsHandlerStab());
            this.handler.Initialize(playlists.Select(p => NonBindableTreePlaylistInfo.ConvertToTreePlaylistInfo(p)).ToList());
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        [TestCase(5)]
        [TestCase(6)]
        [TestCase(7)]
        public void プレイリストの存在をチェックする(int id)
        {
            Assert.IsTrue(this.handler.Contains(id));
        }

        [Test]
        public void プレイリストを削除する()
        {
            this.handler.Remove(1);
            Assert.IsFalse(this.handler.Contains(1));
        }

        [TestCase(1, "一")]
        [TestCase(2, "二")]
        [TestCase(3, "三")]
        [TestCase(4, "四")]
        [TestCase(5, "五")]
        [TestCase(6, "六")]
        [TestCase(7, "七")]
        public void プレイリストを取得する(int id, string name)
        {
            ITreePlaylistInfo? playist = this.handler.GetPlaylist(id);
            Assert.That(playist, Is.Not.Null);
            Assert.That(playist!.Id, Is.EqualTo(id));
            Assert.That(playist.Name.Value, Is.EqualTo(name));
        }

        [Test]
        public void 親プレイリストを取得する()
        {
            ITreePlaylistInfo? parent = this.handler.GetParent(2);

            Assert.IsNotNull(parent);
            Assert.AreEqual(1, parent?.Id);
        }

        [Test]
        public void ツリーを構築する()
        {
            ITreePlaylistInfo tree = this.handler.Playlists.First();

            //子プレイリストの概観チェック
            Assert.AreEqual(2, tree.Children.Count);
            Assert.AreEqual(1, tree.Children[0].Children.Count);
            Assert.AreEqual(3, tree.Children[0].Children[0].Children.Count);

            //子プレイリストの詳細チェック
            Assert.AreEqual(1, tree.Id);
            Assert.AreEqual(2, tree.Children[0].Id);
            Assert.AreEqual(3, tree.Children[1].Id);
            Assert.AreEqual(4, tree.Children[0].Children[0].Id);
            Assert.AreEqual(5, tree.Children[0].Children[0].Children[0].Id);
            Assert.AreEqual(6, tree.Children[0].Children[0].Children[1].Id);
            Assert.AreEqual(7, tree.Children[0].Children[0].Children[2].Id);
        }

        [TestCase(1, false)]
        [TestCase(2, false)]
        [TestCase(3, true)]
        [TestCase(4, true)]
        [TestCase(5, false)]
        [TestCase(6, false)]
        [TestCase(7, true)]
        public void 最後の子プレイリストであることを確認する(int id, bool result)
        {
            bool isLastChild = this.handler.IsLastChild(id);
            Assert.AreEqual(result, isLastChild);

        }

        [Test]
        public void 親プレイリストのリストを取得する()
        {
            List<string> ancester = this.handler!.GetListOfAncestor(5);

            Assert.That(ancester[0], Is.EqualTo("一"));
            Assert.That(ancester[1], Is.EqualTo("二"));
            Assert.That(ancester[2], Is.EqualTo("四"));
            Assert.That(ancester[3], Is.EqualTo("五"));
        }
    }

    class VideoFilterUnitTest
    {
        private IVideoFilter? videoFilter;

        private List<IListVideoInfo>? videos;

        [SetUp]
        public void SetUp()
        {
            this.videoFilter = new VideoFilter();
            this.videos = new List<IListVideoInfo>();
            var video1 = new NonBindableListVideoInfo() { Tags = new List<string>() { "東方", "テスト" } };
            var video2 = new NonBindableListVideoInfo() { Tags = new List<string>() { "MMD", "テスト" } };
            var video3 = new NonBindableListVideoInfo() { Tags = new List<string>() { "IDOL_M@STER", "テスト" } };
            var video4 = new NonBindableListVideoInfo() { Tags = new List<string>() { "最古の動画", "sm9", "テスト" } };
            var video5 = new NonBindableListVideoInfo() { Tags = new List<string>() { "タグ", "テスト" } };

            video1.Title.Value = "テスト東方";
            video1.Id.Value = 1;
            video2.Title.Value = "テストMMD";
            video2.Id.Value = 2;
            video3.Title.Value = "テストIDOL M@STER";
            video3.Id.Value = 3;
            video4.Title.Value = "レッツゴー!陰陽師";
            video4.Id.Value = 4;
            video5.Title.Value = "タグテスト";
            video5.Id.Value = 5;
            this.videos.AddRange(new List<IListVideoInfo>() { video1, video2, video3, video4, video5 });
        }

        [TestCase("テスト", 5)]
        [TestCase("東方", 1)]
        [TestCase("sm9", 1)]
        [TestCase("タグ", 1)]
        public void キーワードで検索する(string keyword, int expectedCount)
        {
            var result = this.videoFilter!.FilterVideos(keyword, this.videos!);
            Assert.AreEqual(expectedCount, result.Count());
        }

    }
}
