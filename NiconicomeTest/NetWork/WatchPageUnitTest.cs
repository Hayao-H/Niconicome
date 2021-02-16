using NUnit.Framework;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Niconico.Watch;
using Microsoft.Extensions.DependencyInjection;
using Niconicome.Models.Domain.Utils;
using Niconicome.Models.Domain.Niconico.Download.Video;
using Niconicome.Models.Domain.Niconico;
using Niconicome.Extensions.System.List;
using Niconicome.Models.Playlist;
using NiconicomeTest.Stabs.Models.Domain.Niconico.NicoHttpStabs;
using System.Net.Http;

namespace NiconicomeTest.NetWork
{
    [TestFixture]
    class WatchPageInfoUnitTest
    {
        private WatchInfohandler? info;

        private readonly string title = "新・豪血寺一族  -煩悩解放 - レッツゴー！陰陽師";

        [OneTimeSetUp]
        public void SetUp()
        {
            var content = new StringContent(Properties.Resources.sm9_page);
            var http = new NicoHttpStab(content, content);
            this.info = new WatchInfohandler(http, new WatchPageHtmlPaserStab(this.title));
        }

        [Test]
        public async Task タイトル()
        {
            if (this.info == null) return;
            IDomainVideoInfo video = await this.info.GetVideoInfoAsync("sm9", WatchInfoOptions.Default);
            Assert.AreEqual(this.title, video?.Title);
        }
    }

    [TestFixture]
    class WatchPageHtmlParserUnittest
    {
        private IWatchPageHtmlParser? parser;

        [OneTimeSetUp]
        public void SetUp()
        {
            this.parser = new WatchPageHtmlParser();
        }

        [Test]
        public void 非暗号化動画のページをパースする()
        {
            IDmcInfo? info = this.parser?.GetDmcInfo(Properties.Resources.Niconico_Onmyoji,"sm9", WatchInfoOptions.Default);

            //nullチェック
            Assert.IsNotNull(info);

            //タイトル
            Assert.AreEqual("新・豪血寺一族 -煩悩解放 - レッツゴー！陰陽師", info?.Title);

            //ID
            Assert.AreEqual("sm9", info?.Id);

            //サムネイル
            Assert.AreEqual("https://img.cdn.nimg.jp/s/nicovideo/thumbnails/9/9.original/a960x540l?key=00208e6cfc9ead0306cfa0291c2734a15cdea8f58df302b5ac8ed05428ef1190", info?.ThumbInfo?.Large);
            Assert.AreEqual("https://nicovideo.cdn.nimg.jp/thumbnails/9/9", info?.ThumbInfo?.Normal);

        }

        [Test]
        public void 暗号化動画ページをパースする()
        {
            IDmcInfo? info = this.parser?.GetDmcInfo(Properties.Resources.Niconico_Kakushigoto, "so36605534", WatchInfoOptions.Default);

            //nullチェック
            Assert.IsNotNull(info);

            //タイトル
            Assert.AreEqual("かくしごと 第1話「かくしごと」 「ねがいごと」", info?.Title);

            //ID
            Assert.AreEqual("so36605534", info?.Id);

            //サムネイル
            Assert.AreEqual("https://img.cdn.nimg.jp/s/nicovideo/thumbnails/36605534/36605534.47637468.original/a960x540l?key=cd9deaaa13ba3301b5a8e24dafa6fceea21b60a7ca9c11fcc9abcdf3198f8978", info?.ThumbInfo?.Large);
            Assert.AreEqual("https://nicovideo.cdn.nimg.jp/thumbnails/36605534/36605534.47637468", info?.ThumbInfo?.Normal);

        }
    }

    [TestFixture]
    class DmcDatahandlerUnitTest
    {
        private IDmcDataHandler? handler;

        [SetUp]
        public void SetUp()
        {
            var content = new StringContent(Properties.Resources.sm9_page);
            var http = new NicoHttpStab(content, content);
            this.handler = new DmcDataHandler(http);
        }

        [Test]
        public void Nullチェックをテストする()
        {

            Exception? exception = null;

            try
            {
                this.handler!.GetPostData(new DomainVideoInfo());
            }
            catch (Exception e)
            {
                exception = e;
            }

            Assert.IsNotNull(exception);
        }

    }


    class WatchPageHtmlPaserStab : IWatchPageHtmlParser
    {
        public WatchPageHtmlPaserStab(string title)
        {
            this.title = title;
            this.HasJsDataElement = true;
        }

        private readonly string title;

        public bool HasJsDataElement { get; init; }

        public IDmcInfo GetDmcInfo(string source,string niconicoId, WatchInfoOptions options)
        {
            return new DmcInfo()
            {
                Id = "sm9",
                Title = this.title
            };
        }

    }
}
