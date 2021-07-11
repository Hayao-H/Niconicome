using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Local.Addons.Core;
using Niconicome.Models.Domain.Local.Addons.Core.Installer;
using Niconicome.Models.Domain.Local.Addons.Core.Permisson;
using Niconicome.Models.Domain.Local.Addons.Manifest.V1;
using Niconicome.Models.Domain.Niconico.Net.Json;
using Niconicome.Models.Helper.Result;
using Niconicome.Models.Helper.Result.Generic;
using NiconicomeTest.Stabs.Models.Domain.Local.IO;
using NiconicomeTest.Stabs.Models.Domain.Utils;
using NUnit.Framework;

namespace NiconicomeTest.Local.Addons.Core.Installer
{
    class ManfestLoaderUnitTest
    {
        private IManifestLoader? manifestLoader;

        [SetUp]
        public void SetUp()
        {
            this.manifestLoader = new ManifestLoader(new NicoFileIOMock(() => true, () => Properties.Resources.ManifestV1), new LoggerStab(), new PermissionsHandler());
        }

        [Test]
        public void マニフェストを読み込む()
        {
            IAttemptResult<AddonInfomation> result = this.manifestLoader!.LoadManifest("");

            Assert.That(result.IsSucceeded,Is.True);
            Assert.That(result.Data, Is.Not.Null);
            Assert.That(result.Data!.Name.Value, Is.EqualTo("Test"));
            Assert.That(result.Data!.Author.Value, Is.EqualTo("Test"));
            Assert.That(result.Data!.Description.Value, Is.EqualTo("Test"));
            Assert.That(result.Data!.Identifier.Value, Is.EqualTo("Test_ID"));
            Assert.That(result.Data!.Version.Value.ToString(), Is.EqualTo("1.0.0"));

        }
    }
}
