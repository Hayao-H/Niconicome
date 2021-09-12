using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Local.Addons.Core.AutoUpdate.Github;
using Niconicome.Models.Domain.Network;
using Niconicome.Models.Helper.Result;
using NiconicomeTest.Stabs.Models.Domain.Niconico.NicoHttpStabs;
using NiconicomeTest.Stabs.Models.Domain.Utils;
using NUnit.Framework;

namespace NiconicomeTest.Local.Addons.Core.AutoUpdate.Github
{
    [TestFixture]
    class GithubReleaseUnitTest
    {
        private IReleaseChecker? checker;

        [SetUp]
        public void SetUp()
        {
            var content = new StringContent(Properties.Resources.GithubReleases);
            var http = new NicoHttpStab(content,content);
            this.checker = new ReleaseChecker(http, new NetWorkHelper(), new LoggerStab());
        }

        [Test]
        public async Task リリースを取得する()
        {
            IAttemptResult<Release> result = await this.checker!.GetTheLatestAsync("","");

            Assert.That(result.IsSucceeded, Is.True);
            Assert.That(result.Data, Is.Not.Null);
            Assert.That(result.Data!.Name, Is.EqualTo("v0.7.1"));
        }

        [Test]
        public async Task バージョンを確認する()
        {
           Release  release = (await this.checker!.GetTheLatestAsync("", "")).Data!;

            IAttemptResult<Version> result = this.checker!.GetVersion(release);

            Assert.That(result.IsSucceeded, Is.True);
            Assert.That(result.Data, Is.Not.Null);
            Assert.That(result.Data!.Major, Is.EqualTo(0));
            Assert.That(result.Data!.Minor, Is.EqualTo(7));
            Assert.That(result.Data!.Build, Is.EqualTo(1));
        }

        [Test]
        public async Task ファイルを確認する()
        {
            Release release = (await this.checker!.GetTheLatestAsync("", "")).Data!;
            IAttemptResult<string> result = this.checker!.GetAssetUrl(@"^niconicome-win-x64-self-contained.*$", release);

            Assert.That(result.IsSucceeded, Is.True);
            Assert.That(result.Data!, Is.EqualTo("https://api.github.com/repos/Hayao-H/Niconicome/releases/assets/39480024"));
        }
    }
}
