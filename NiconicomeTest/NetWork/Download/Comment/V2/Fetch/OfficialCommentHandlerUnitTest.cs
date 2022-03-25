using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Network;
using Niconicome.Models.Domain.Niconico.Download.Comment.V2.Fetch;
using Niconicome.Models.Helper.Result;
using NiconicomeTest.Stabs.Models.Domain.Niconico.NicoHttpStabs;
using NiconicomeTest.Stabs.Models.Domain.Utils;
using NUnit.Framework;
using CFetch = Niconicome.Models.Domain.Niconico.Download.Comment.V2.Fetch;

namespace NiconicomeTest.NetWork.Download.Comment.V2.Fetch
{
    internal class OfficialCommentHandlerUnitTest
    {
        private NicoHttpStab? _http;

        private IOfficialCommentHandler? _handler;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            this._http = new NicoHttpStab(new StringContent(string.Empty), new StringContent(string.Empty));
            this._handler = new OfficialCommentHandler(this._http, new LoggerStub(), new NetWorkHelper());
        }

        [TestCase("waybackkey=123456.abcdefg",true, "123456.abcdefg")]
        [TestCase("",false, null)]
        [TestCase("waybackkey=",false, null)]
        [TestCase("waybackkey",false, null)]
        public async Task WaybackKey取得テスト(string content, bool expected, string? expectedKey)
        {
            this._http!.GetContent = new StringContent(content);

            IAttemptResult<CFetch::WayBackKey> result = await this._handler!.GetWayBackKeyAsync("");

            Assert.That(result.IsSucceeded, Is.EqualTo(expected));

            if (expectedKey is null)
            {
                Assert.That(result.Data, Is.Null);
            }
            else
            {
                Assert.That(result.Data!.Key, Is.EqualTo(expectedKey));

            }
        }
    }
}
