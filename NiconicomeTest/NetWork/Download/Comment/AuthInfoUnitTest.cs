using NUnit.Framework;
using Niconicome.Models.Domain.Niconico.Download.Comment;
using NiconicomeTest.Stabs.Models.Domain.Niconico.NicoHttpStabs;
using System.Net.Http;
using System.Threading.Tasks;

namespace NiconicomeTest.NetWork.Download.Comment
{
    [TestFixture]
    class AuthInfoUnitTest
    {
        [Test]
        public async Task 取得に成功した場合()
        {
            var content = new StringContent("threadkey=1611461521.GIWx_XyM-juZoqSIdvEOU9rIYy8&force_184=1");
            var http = new NicoHttpStab(content, content);
            var auth = new OfficialVideoUtils(http);
            var info = await auth.GetAuthInfoAsync("1234");

            Assert.That(info.Force184, Is.EqualTo("1"));
            Assert.That(info.ThreadKey, Is.EqualTo("1611461521.GIWx_XyM-juZoqSIdvEOU9rIYy8"));
        }

        [Test]
        public void 取得に失敗した場合()
        {
            var content = new StringContent("error");
            var http = new NicoHttpStab(content, content);
            var auth = new OfficialVideoUtils(http);

            Assert.That(async () => await auth.GetAuthInfoAsync("1234"), Throws.TypeOf<HttpRequestException>())
;
        }
    }
}
