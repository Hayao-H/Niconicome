using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Playlist;
using STypes = Niconicome.Models.Domain.Local.Store.Types;

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
            var childPlaylists = new List<STypes.Playlist>() { new STypes.Playlist() { Id = 1 }, new STypes.Playlist() { Id = 2 }, new STypes.Playlist() { Id = 3 } };
            var playlistInfo = NonBindableTreePlaylistInfo.ConvertToTreePlaylistInfo(storeData, childPlaylists);

            //プレイリスト名
            Assert.AreEqual("テスト", storeData.PlaylistName);
            //ID
            Assert.AreEqual(playlistInfo.Id, 1);
            //親のID
            Assert.AreEqual(playlistInfo.ParentId, 1);
            //ルートフラグ
            Assert.IsTrue(playlistInfo.IsRoot);
            //子プレイリスト
            Assert.AreEqual(1, playlistInfo.ChildrensIds[0]);
            Assert.AreEqual(2, playlistInfo.ChildrensIds[1]);
            Assert.AreEqual(3, playlistInfo.ChildrensIds[2]);
        }
    }

    [TestFixture]
    class PlaylistInfoHandlerUnitTest
    {
        private ITreePlaylistInfoHandler handler = new TreePlaylistInfoHandler();

        [SetUp]
        public void SetUp()
        {
            ///・名前
            ///各IDを漢数字に置き換えたもの
            ///・ツリー構造
            ///親(1)─子(2)─子(4)─子(5)
            ///     └子(3)       ┠子(6)
            ///                   └子(7)
            var first = new STypes::Playlist() { Id = 1, IsRoot = true, PlaylistName = "一" };
            var second = new STypes::Playlist() { Id = 2, PlaylistName = "二" };
            var third = new STypes::Playlist() { Id = 3, PlaylistName = "三" };
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
            this.handler = new TreePlaylistInfoHandler();
            this.handler.AddRange(playlists.Select(p => NonBindableTreePlaylistInfo.ConvertToTreePlaylistInfo(p, playlists.Where(pl => pl?.ParentPlaylist?.Id == p.Id))));
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
        public void プレイリストを追加する()
        {
            this.handler.Add(new NonBindableTreePlaylistInfo() { Id = 8 });
            Assert.IsTrue(this.handler.Contains(8));
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
            Assert.IsNotNull(playist);
            Assert.AreEqual(id, playist?.Id);
            Assert.AreEqual(name, playist?.Name);
        }

        [Test]
        public void 親プレイリストを取得する()
        {
            ITreePlaylistInfo? parent = this.handler.GetParent(2);

            Assert.IsNotNull(parent);
            Assert.AreEqual(1, parent?.Id);
        }

        [Test]
        public void ルートプレイリストを取得する()
        {
            ITreePlaylistInfo root = this.handler.GetRoot();
            Assert.AreEqual(1, root.Id);
        }

        [Test]
        public void ツリーを構築する()
        {
            ITreePlaylistInfo tree = this.handler.GetTree();

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
    }

    class VideoFilterUnitTest
    {
        private IVideoFilter? videoFilter;

        private List<ITreeVideoInfo>? videos;

        [SetUp]
        public void SetUp()
        {
            this.videoFilter = new VideoFilter();
            this.videos = new List<ITreeVideoInfo>();
            var video1 = new BindableTreeVideoInfo() { Title = "テスト東方", Tags = new List<string>() { "東方", "テスト" }, Id = 1 };
            var video2 = new BindableTreeVideoInfo() { Title = "テストMMD", Tags = new List<string>() { "MMD", "テスト" }, Id = 2 };
            var video3 = new BindableTreeVideoInfo() { Title = "テストIDOL M@STER", Tags = new List<string>() { "IDOL_M@STER", "テスト" }, Id = 3 };
            var video4 = new BindableTreeVideoInfo() { Title = "レッツゴー!陰陽師", Tags = new List<string>() { "最古の動画", "sm9", "テスト" }, Id = 4 };
            var video5 = new BindableTreeVideoInfo() { Title = "タグテスト", Tags = new List<string>() { "タグ", "テスト" }, Id = 5 };
            this.videos.AddRange(new List<ITreeVideoInfo>() { video1, video2, video3, video4, video5 });
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
