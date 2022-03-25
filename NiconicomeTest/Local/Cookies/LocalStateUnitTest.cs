using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Local.LocalFile;
using Niconicome.Models.Domain.Local.SQLite;
using NiconicomeTest.Stabs.Models.Domain.Utils;
using NUnit.Framework;

namespace NiconicomeTest.Local.Cookies
{
    class LocalStateUnitTest
    {
        private CookieJsonLoader? loader;

        [SetUp]
        public void SetUp()
        {
            this.loader = new CookieJsonLoader(new LoggerStub());
        }

        [Test]
        public void ファイル名を取得()
        {
            var name = this.loader!.GetJsonPath(CookieType.Webview2);

            Assert.That(name, Is.EqualTo(@"Niconicome.exe.WebView2\EBWebView\Local State"));
        }

    }
}
