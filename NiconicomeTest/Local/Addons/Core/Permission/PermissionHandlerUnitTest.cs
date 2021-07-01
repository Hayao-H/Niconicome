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

        [TestCase("hook", true)]
        [TestCase("fuga", false)]
        public void 権限を確認する(string name, bool expectedResult)
        {
            bool result = this.permissionsHandler!.IsKnownPermission(name);

            Assert.That(result, Is.EqualTo(expectedResult));
        }
    }
}
