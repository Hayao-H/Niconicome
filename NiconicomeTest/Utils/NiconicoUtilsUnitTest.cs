using NUnit.Framework;
using System;
using Niconicome.Models.Domain.Utils;
using Niconicome.Models.Playlist.Playlist;
using Niconicome.Models.Domain.Niconico.Watch;
using Niconicome.Models.Domain.Niconico.Search;
using Niconicome.Models.Domain.Niconico.Video.Infomations;

namespace NiconicomeTest.Utils
{
    [TestFixture]
    class NiconicoUtilsUnitTest
    {

        private INiconicoUtils? utils;

        /// <summary>
        /// 初期化
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            this.utils = new NiconicoUtils();
        }

        [Test]
        public void 動画IDを抽出する()
        {
            var source = "sm9はニコニコ動画における最古の動画です。また、一大ジャンルである東方Projectではsm8628149(Bad Apple)が有名です。soではじまる動画、例えばso1234はチャンネル動画、nmで始まる動画、\n例えばnm1234はニコニコムービーメーカーで作成された動画です。最近では123456のように数字のみで構成される動画IDも存在します。";

            var result = this.utils?.GetNiconicoIdsFromText(source);

            Assert.IsNotNull(result);

            if (result is null) return;

            Assert.AreEqual(result.Count, 5);
            Assert.AreEqual("sm9", result[0]);
            Assert.AreEqual("sm8628149", result[1]);
            Assert.AreEqual("so1234", result[2]);
            Assert.AreEqual("nm1234", result[3]);
            Assert.AreEqual("123456", result[4]);
        }

        [TestCase("[123]hoge.mp4", "123")]
        [TestCase("[sm123]hoge", "sm123")]
        public void ファイル名からIDを取得する(string filename, string id)
        {
            var result = this.utils!.GetIdFromFIleName("[<id>]<title>", filename);
            Assert.That(result, Is.EqualTo(id));
        }

        [TestCase("[123]hoge.mp4", "123")]
        [TestCase("[sm123]hoge", "sm123")]
        [TestCase("sm123hoge", "sm123")]
        [TestCase("hoge(nm2)", "nm2")]
        [TestCase("ssssso123hoge", "so123")]
        public void フォーマットなしでファイル名からIDを取得する(string filename, string id)
        {
            var result = this.utils!.GetIdFromFIleName(filename);
            Assert.That(result, Is.EqualTo(id));
        }

        [TestCase("sm9", true)]
        [TestCase("so123", true)]
        [TestCase("123", true)]
        [TestCase("nm9", true)]
        [TestCase("nm123", true)]
        [TestCase("hsm9", false)]
        [TestCase("sm91a", false)]
        [TestCase("a12", false)]
        [TestCase("12d", false)]
        public void ニコニコのIDをテストする(string testString, bool expectedResult)
        {
            var result = this.utils!.IsNiconicoID(testString);

            Assert.That(result, Is.EqualTo(expectedResult));
        }

        [TestCase("https://www.nicovideo.jp/user/000/mylist/000?ref=pc_userpage_menu", RemoteType.Mylist)]
        [TestCase("https://www.nicovideo.jp/my/watchlater?ref=pc_mypage_menu", RemoteType.WatchLater)]
        [TestCase("https://ch.nicovideo.jp/elfenlied", RemoteType.Channel)]
        [TestCase("https://www.nicovideo.jp/user/000/video?ref=pc_userpage_menud", RemoteType.UserVideos)]
        [TestCase("https://www.nicovideo.jp/watch/sm9?hello_world", RemoteType.WatchPage)]
        [TestCase("https://www.nicovideo.jp/watch/sm9", RemoteType.WatchPage)]
        [TestCase("https://www.nicovideo.jp/series/123", RemoteType.Series)]
        public void リモートプレイリストの種別を取得する(string url, RemoteType expectedType)
        {
            var type = this.utils!.GetRemoteType(url);

            Assert.That(expectedType, Is.EqualTo(type));
        }

        [TestCase("https://www.nicovideo.jp/user/000/mylist/000?ref=pc_userpage_menu", RemoteType.Mylist, "000")]
        [TestCase("https://www.nicovideo.jp/user/000/mylist/000/?ref=pc_userpage_menu", RemoteType.Mylist, "000")]
        [TestCase("https://ch.nicovideo.jp/elfenlied", RemoteType.Channel, "elfenlied")]
        [TestCase("https://ch.nicovideo.jp/elfenlied/", RemoteType.Channel, "elfenlied")]
        [TestCase("https://www.nicovideo.jp/user/000/video?ref=pc_userpage_menud", RemoteType.UserVideos, "000")]
        [TestCase("https://www.nicovideo.jp/user/000/video/?ref=pc_userpage_menud", RemoteType.UserVideos, "000")]
        [TestCase("https://www.nicovideo.jp/watch/sm9?hello_world", RemoteType.WatchPage, "sm9")]
        [TestCase("https://www.nicovideo.jp/watch/sm9", RemoteType.WatchPage, "sm9")]
        [TestCase("https://www.nicovideo.jp/series/123", RemoteType.Series,"123")]
        public void リモートプレイリストのIDを取得する(string url, RemoteType type, string id)
        {
            var result = this.utils!.GetID(url, type);

            Assert.That(result, Is.EqualTo(id));
        }

        [TestCase("[<id>]<title>", "[sm9]新・豪血寺一族 -煩悩解放 - レッツゴー！陰陽師")]
        [TestCase("[<id>]<title>（<owner>）", "[sm9]新・豪血寺一族 -煩悩解放 - レッツゴー！陰陽師（中の）")]
        [TestCase("[<id>]<title>（<ownerId>）", "[sm9]新・豪血寺一族 -煩悩解放 - レッツゴー！陰陽師（1）")]
        [TestCase("[<id>]<title>（<uploadedon>）", "[sm9]新・豪血寺一族 -煩悩解放 - レッツゴー！陰陽師（2007-03-06 00-33-00）")]
        [TestCase("[<id>]<title>（<downloadon>）", "[sm9]新・豪血寺一族 -煩悩解放 - レッツゴー！陰陽師（2021-03-26 20-30-00）")]
        [TestCase("[<id>]<title>（<uploadedon:yyyy年MM月dd日 HH時mm分ss秒>）", "[sm9]新・豪血寺一族 -煩悩解放 - レッツゴー！陰陽師（2007年03月06日 00時33分00秒）")]
        [TestCase("[<id>]<title>（<downloadon:yyyy年MM月dd日 HH時mm分ss秒>）", "[sm9]新・豪血寺一族 -煩悩解放 - レッツゴー！陰陽師（2021年03月26日 20時30分00秒）")]
        public void 動画ファイル名を取得する(string format, string expectedResult)
        {
            var dt = DateTime.Parse("2007-03-06T00:33:00");
            var dt2 = DateTime.Parse("2021-03-26T20:30:00");
            var dmc = new DmcInfo()
            {
                UploadedOn = dt,
                DownloadStartedOn = dt2,
                Title = "新・豪血寺一族 -煩悩解放 - レッツゴー！陰陽師",
                Owner = "中の",
                Id = "sm9",
                OwnerID = 1,
            };

            var result = this.utils!.GetFileName(format, dmc, string.Empty, true);

            Assert.That(result, Is.EqualTo(expectedResult));
        }

        [TestCase(@"https://www.nicovideo.jp/tag/%E6%9D%B1%E6%96%B9?sort=v&order=d", "東方", SearchType.Tag, Sort.ViewCount, false)]
        [TestCase(@"https://www.nicovideo.jp/tag/%E6%9D%B1%E6%96%B9?sort=f&order=d", "東方", SearchType.Tag, Sort.UploadedTime, false)]
        [TestCase(@"https://www.nicovideo.jp/tag/%E6%9D%B1%E6%96%B9?sort=m&order=d", "東方", SearchType.Tag, Sort.MylistCount, false)]
        [TestCase(@"https://www.nicovideo.jp/tag/%E6%9D%B1%E6%96%B9?sort=n&order=d", "東方", SearchType.Tag, Sort.LastCommentTime, false)]
        [TestCase(@"https://www.nicovideo.jp/tag/%E6%9D%B1%E6%96%B9?sort=r&order=d", "東方", SearchType.Tag, Sort.CommentCount, false)]
        [TestCase(@"https://www.nicovideo.jp/tag/%E6%9D%B1%E6%96%B9?sort=l&order=d", "東方", SearchType.Tag, Sort.Length, false)]
        [TestCase(@"https://www.nicovideo.jp/tag/%E6%9D%B1%E6%96%B9?sort=v&order=a", "東方", SearchType.Tag, Sort.ViewCount, true)]
        [TestCase(@"https://www.nicovideo.jp/tag/%E6%9D%B1%E6%96%B9?sort=f&order=a", "東方", SearchType.Tag, Sort.UploadedTime, true)]
        [TestCase(@"https://www.nicovideo.jp/tag/%E6%9D%B1%E6%96%B9?sort=m&order=a", "東方", SearchType.Tag, Sort.MylistCount, true)]
        [TestCase(@"https://www.nicovideo.jp/tag/%E6%9D%B1%E6%96%B9?sort=n&order=a", "東方", SearchType.Tag, Sort.LastCommentTime, true)]
        [TestCase(@"https://www.nicovideo.jp/tag/%E6%9D%B1%E6%96%B9?sort=r&order=a", "東方", SearchType.Tag, Sort.CommentCount, true)]
        [TestCase(@"https://www.nicovideo.jp/tag/%E6%9D%B1%E6%96%B9?sort=l&order=a", "東方", SearchType.Tag, Sort.Length, true)]
        [TestCase(@"https://www.nicovideo.jp/search/%E6%9D%B1%E6%96%B9?sort=v&order=d", "東方", SearchType.Keyword, Sort.ViewCount, false)]
        [TestCase(@"https://www.nicovideo.jp/search/%E6%9D%B1%E6%96%B9?sort=f&order=d", "東方", SearchType.Keyword, Sort.UploadedTime, false)]
        [TestCase(@"https://www.nicovideo.jp/search/%E6%9D%B1%E6%96%B9?sort=m&order=d", "東方", SearchType.Keyword, Sort.MylistCount, false)]
        [TestCase(@"https://www.nicovideo.jp/search/%E6%9D%B1%E6%96%B9?sort=n&order=d", "東方", SearchType.Keyword, Sort.LastCommentTime, false)]
        [TestCase(@"https://www.nicovideo.jp/search/%E6%9D%B1%E6%96%B9?sort=r&order=d", "東方", SearchType.Keyword, Sort.CommentCount, false)]
        [TestCase(@"https://www.nicovideo.jp/search/%E6%9D%B1%E6%96%B9?sort=l&order=d", "東方", SearchType.Keyword, Sort.Length, false)]
        [TestCase(@"https://www.nicovideo.jp/search/%E6%9D%B1%E6%96%B9?sort=v&order=a", "東方", SearchType.Keyword, Sort.ViewCount, true)]
        [TestCase(@"https://www.nicovideo.jp/search/%E6%9D%B1%E6%96%B9?sort=f&order=a", "東方", SearchType.Keyword, Sort.UploadedTime, true)]
        [TestCase(@"https://www.nicovideo.jp/search/%E6%9D%B1%E6%96%B9?sort=m&order=a", "東方", SearchType.Keyword, Sort.MylistCount, true)]
        [TestCase(@"https://www.nicovideo.jp/search/%E6%9D%B1%E6%96%B9?sort=n&order=a", "東方", SearchType.Keyword, Sort.LastCommentTime, true)]
        [TestCase(@"https://www.nicovideo.jp/search/%E6%9D%B1%E6%96%B9?sort=r&order=a", "東方", SearchType.Keyword, Sort.CommentCount, true)]
        [TestCase(@"https://www.nicovideo.jp/search/%E6%9D%B1%E6%96%B9?sort=l&order=a", "東方", SearchType.Keyword, Sort.Length, true)]
        public void 検索URLからクエリを取得する(string url, string kw, SearchType searchType, Sort sort, bool isAscending)
        {
            var result = this.utils!.GetQueryFromUrl(url);

            Assert.That(result.Query, Is.EqualTo(kw));
            Assert.That(result.SearchType, Is.EqualTo(searchType));
            Assert.That(result.SortOption.Sort, Is.EqualTo(sort));
            Assert.That(result.SortOption.IsAscending, Is.EqualTo(isAscending));
        }

        [TestCase(@"https://www.nicovideo.jp/tag/%E6%9D%B1%E6%96%B9?sort=h&order=d")]
        [TestCase(@"https://www.nicovideo.jp/tag/%E6%9D%B1%E6%96%B9?sort=h&order=a")]
        public void 対応していない検索URLからクエリを取得する(string url)
        {
            Assert.Throws<ArgumentException>(() => this.utils!.GetQueryFromUrl(url));
        }

        [TestCase(@"https://www.nicovideo.jp/tag/%E6%9D%B1%E6%96%B9?sort=v&order=d", true)]
        [TestCase(@"https://www.nicovideo.jp/search/%E6%9D%B1%E6%96%B9?sort=l&order=a", true)]
        [TestCase(@"hoge", false)]
        [TestCase(@"https://www.google.com", false)]
        public void 検索URLをテストする(string url, bool expected)
        {
            var result = this.utils!.IsSearchUrl(url);
            Assert.That(result, Is.EqualTo(expected));
        }

    }
}
