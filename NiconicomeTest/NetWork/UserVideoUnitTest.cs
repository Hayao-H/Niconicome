using System;
using System.Net.Http;
using System.Threading.Tasks;
using NiconicomeTest.Stabs.Models.Domain.Niconico.NicoHttpStabs;
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
            this.handler = new UVideo::UserVideoHandler(http);
        }

        [Test]
        public async Task 投稿動画を取得する()
        {
            if (this.handler is null) throw new InvalidOperationException();

            var videos = await this.handler.GetVideosAsync("4");

            Assert.AreEqual(19, videos.Count);
        }
    }


}
