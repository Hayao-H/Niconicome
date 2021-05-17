using System.Net.Http;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Network;
using Niconicome.Models.Domain.Niconico.Video.Ichiba;
using Niconicome.Models.Network.Watch;
using NiconicomeTest.Stabs.Models.Domain.Niconico.NicoHttpStabs;
using NUnit.Framework;

namespace NiconicomeTest.NetWork.Niconico.Videos.Icihba
{
    class NiconicoIchibaHandlerUnitTest
    {

        private INiconicoIchibaHandler? niconicoIchibaHandler;

        [SetUp]
        public void SetUp()
        {
            var res = new StringContent(Properties.Resources.IchibaResponse);
            var http = new NicoHttpStab(res, res);
            this.niconicoIchibaHandler = new NiconicoIchibaHandler(http, new NetWorkHelper(), new IchibaHtmlParser());
        }

        [Test]
        public async Task 陰陽師を解析する()
        {
            var result = await this.niconicoIchibaHandler!.GetIchibaInfo("sm9");
            Assert.That(result.IsSucceeded, Is.True);
            Assert.That(result.Data, Is.Not.Null);
            Assert.That(result.Data!.IchibaItems.Count, Is.EqualTo(16));
        }
    }
}
