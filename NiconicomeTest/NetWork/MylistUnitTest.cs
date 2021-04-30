using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Niconicome.Models.Playlist;
using NiconicomeTest.MyResources;
using NiconicomeTest.Stabs.Models.Domain.Niconico.NicoHttpStabs;
using NiconicomeTest.Stabs.Models.Domain.Utils;
using NUnit.Framework;
using Mylist = Niconicome.Models.Domain.Niconico.Mylist;

namespace NiconicomeTest.NetWork
{

    [TestFixture]
    class MylistUnitTest
    {
        Mylist::IMylistHandler? handler;

        [SetUp]
        public void SetUp()
        {
            var content = new StringContent(Properties.Resources.API_Mylist_Get);
            var http = new NicoHttpStab(content,content);
            this.handler = new Mylist::MylistHandler(http,new LoggerStab());
        }

        [Test]
        public async Task マイリストを取得する()
        {
            if (this.handler is null) throw new InvalidOperationException();
            var videos = new List<IListVideoInfo>();

            await this.handler.GetVideosAsync("67607290",videos);

            Assert.AreEqual(100, videos.Count);
        }
    }
    

}
