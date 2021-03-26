using NUnit.Framework;
using System;
using Niconicome.Models.Domain.Utils;
using Niconicome.Models.Playlist;
using Niconicome.Models.Domain.Niconico.Watch;

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
        public void リモートプレイリストのIDを取得する(string url, RemoteType type, string id)
        {
            var result = this.utils!.GetID(url, type);

            Assert.That(result, Is.EqualTo(id));
        }

        [TestCase("[<id>]<title>", "[sm9]新・豪血寺一族 -煩悩解放 - レッツゴー！陰陽師")]
        [TestCase("[<id>]<title>（<owner>）", "[sm9]新・豪血寺一族 -煩悩解放 - レッツゴー！陰陽師（中の）")]
        [TestCase("[<id>]<title>（<uploadedon>）", "[sm9]新・豪血寺一族 -煩悩解放 - レッツゴー！陰陽師（2007／03／06 00：33.00）")]
        [TestCase("[<id>]<title>（<uploadedon:yyyy年MM月dd日 HH時mm分ss秒>）", "[sm9]新・豪血寺一族 -煩悩解放 - レッツゴー！陰陽師（2007年03月06日 00時33分00秒）")]
        public void 動画ファイル名を取得する(string format, string expectedResult)
        {
            var dt = DateTime.Parse("2007-03-06T00:33:00");
            var dmc = new DmcInfo()
            {
                UploadedOn = dt,
                Title = "新・豪血寺一族 -煩悩解放 - レッツゴー！陰陽師",
                Owner = "中の",
                Id = "sm9",
            };

            var result = this.utils!.GetFileName(format, dmc, string.Empty, true);

            Assert.That(result, Is.EqualTo(expectedResult));
        }
    }
}
