using System;
using Niconicome.Models.Domain.Local.Addons.Core.Permisson;
using NUnit.Framework;

namespace NiconicomeTest.Local.Addons.Core.Permission
{
    class PermissionHandlerUnitTest
    {
        private IPermissionsHandler? permissionsHandler;

        [SetUp]
        public void SetUp()
        {
            this.permissionsHandler = new PermissionsHandler();
        }

        [TestCase("hooks", true)]
        [TestCase("fuga", false)]
        [TestCase("session", true)]
        public void 権限を確認する(string name, bool expectedResult)
        {
            bool result = this.permissionsHandler!.IsKnownPermission(name);

            Assert.That(result, Is.EqualTo(expectedResult));
        }

        [TestCase("http://niovideo.jp/*", true)]
        [TestCase("https://niovideo.jp/*", true)]
        [TestCase("https://*.niovideo.jp/*", true)]
        [TestCase("http://www.google.com", false)]
        [TestCase("https://*hoge/", false)]
        public void URLパターンを確認する(string pattern, bool expectedResult)
        {
            bool result = this.permissionsHandler!.IsValidUrlPattern(pattern);

            Assert.That(result, Is.EqualTo(expectedResult));
        }
    }
}
