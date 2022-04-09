using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Niconicome.Models.Playlist;
using NiconicomeTest.Stabs.Models.Domain.Niconico.NicoHttpStabs;
using NiconicomeTest.Stabs.Models.Domain.Utils;
using NiconicomeTest.Stabs.Models.Playlist;
using NUnit.Framework;
using UVideo = Niconicome.Models.Domain.Niconico.Video;

namespace NiconicomeTest.NetWork
{

    [TestFixture]
    class UserVideoUnitTest
    {
        UVideo::IUserVideoHandler? handler;

        [SetUp]
        public void SetUp()
        {
            var content = new StringContent(Properties.Resources.User_Video_Response);
            var http = new NicoHttpStab(content, content);
            this.handler = new UVideo::UserVideoHandler(http, new LoggerStub(),new VideoInfoContainerStab());
        }

        [Test]
        public async Task 投稿動画を取得する()
        {
            if (this.handler is null) throw new InvalidOperationException();
            var videos = new List<IListVideoInfo>();


            await this.handler.GetVideosAsync("4", videos);

            Assert.AreEqual(19, videos.Count);
        }
    }


}
