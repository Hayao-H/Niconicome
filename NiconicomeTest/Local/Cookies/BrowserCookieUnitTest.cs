using Niconicome.Models.Domain.Local.SQLite;
using NiconicomeTest.Stabs.Models.Domain.Utils;
using NUnit.Framework;

namespace NiconicomeTest.Local.Cookies
{
    class BrowserCookieUnitTest
    {
        private ISqliteCookieLoader? loader;

        [SetUp]
        public void SetUp()
        {
            this.loader = new SqliteCookieLoader(new SQliteLoader(new LoggerStub()));
        }

        [Test]
        public void ファイル名を取得()
        {
            var wv2 = this.loader!.GetCookiePath(CookieType.Webview2);

            Assert.That(wv2, Is.EqualTo(@"Niconicome.exe.WebView2\EBWebView\Default\Network\Cookies"));
        }
    }
}
