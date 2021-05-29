using System.Collections.Generic;
using System.Linq;
using Niconicome.Models.Domain.Local.External.Software.Mozilla.Firefox;
using NiconicomeTest.Stabs.Models.Domain.Local.IO;
using NUnit.Framework;

namespace NiconicomeTest.Local.External.Software.Mozilla.Firefox
{
    class FirefoxProfileManagerUnitTest
    {
        private IFirefoxProfileManager? manager;

        [SetUp]
        public void SetUp()
        {
            var stab = new NicoDirectoryIOMock(p => true, (_, _, _) => new List<string>(), (_, _, _) => new List<string>() { "abcd64.default-release", "abcd64.dev-edition-default", "abcd64.default", "abcd64.default-nightly" });
            this.manager = new FirefoxProfileManager(stab);
        }

        [Test]
        public void すべてのプロファイルを取得する()
        {
            var profiles = this.manager!.GetAllProfiles();
            Assert.That(profiles.Count(), Is.EqualTo(4));
        }

        [TestCase("abcd64.default-release", true)]
        [TestCase("abcd64.dev-edition-default", true)]
        [TestCase("abcd64.default", true)]
        [TestCase("abcd64.default-nightly", true)]
        [TestCase("False", false)]
        public void プロファイルの存在を確認する(string name, bool expectedResult)
        {
            var result = this.manager!.HasProfile(name);
            Assert.That(result, Is.EqualTo(expectedResult));
        }

        [Test]
        public void プロファイルを取得する()
        {
            var profile = this.manager!.GetProfile("abcd64.default-release");
            Assert.That(profile.ProfileName, Is.EqualTo("default-release"));
        }
    }
}
