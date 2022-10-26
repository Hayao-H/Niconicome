using System.Collections.Generic;
using Niconicome.Models.Domain.Local.Addons.Core.V2.Utils;
using NUnit.Framework;

namespace NiconicomeTest.Local.Addons.Core.Utils
{
    [TestFixture]
    internal class HostPermissionsHandlerUnitTest
    {
        private IHostPermissionsHandler? hostPermissionsHandler;

        [SetUp]
        public void SetUp()
        {
            var hosts = new List<string>() { "https://nicovideo.jp", "http://nicovideo.jp", "https://*.nicovideo.jp/*", "http://*.nicovideo.jp/*", "https://nicovideo.jp/*", "http://nicovideo.jp/*" };
            this.hostPermissionsHandler = new HostPermissionsHandler();
            this.hostPermissionsHandler.Initialize(hosts);
        }

        [TestCase("https://nicovideo.jp",true)]
        [TestCase("http://nicovideo.jp",true)]
        [TestCase("https://nicovideo.jp/watch/sm9",true)]
        [TestCase("http://nicovideo.jp/watch/sm9", true)]
        [TestCase("https://dmc.nicovideo.jp/test.m3u8",true)]
        [TestCase("http://dmc.nicovideo.jp/test.m3u8", true)]
        [TestCase("https://google.com", false)]
        [TestCase("Hello World!!", false)]
        public void ホストをチェックする(string url,bool expectedResult)
        {
            bool result = this.hostPermissionsHandler!.CanAccess(url);
            Assert.That(result, Is.EqualTo(expectedResult));
        }
    }
}
