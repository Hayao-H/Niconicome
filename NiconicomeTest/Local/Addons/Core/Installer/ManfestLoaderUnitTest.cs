using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Local.Addons.Core;
using Niconicome.Models.Domain.Local.Addons.Core.V2.Engine.Infomation;
using Niconicome.Models.Domain.Local.Addons.Core.V2.Permisson;
using Niconicome.Models.Domain.Local.Addons.Manifest.V1;
using Niconicome.Models.Domain.Niconico.Net.Json;
using Niconicome.Models.Helper.Result;
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
            this.manifestLoader = new ManifestLoader(new NicoFileIOMock(() => true, () => Properties.Resources.ManifestV2), new LoggerStub(), new PermissionsHandler());
        }

        [Test]
        public void マニフェスト作成()
        {
            var manifest = new Manifest();
            JsonSerializerOptions option = JsonParser.DefaultOption;
            option.WriteIndented = true;
            string serialized = JsonParser.Serialize(manifest, option);
            Console.WriteLine(serialized);
        }

        [Test]
        public void マニフェストを読み込む()
        {
            IAttemptResult<IAddonInfomation> result = this.manifestLoader!.LoadManifest("","");

            Assert.That(result.IsSucceeded, Is.True);
            Assert.That(result.Data, Is.Not.Null);
            Assert.That(result.Data!.Name, Is.EqualTo("Test"));
            Assert.That(result.Data!.Author, Is.EqualTo("Test"));
            Assert.That(result.Data!.Description, Is.EqualTo("Test"));
            Assert.That(result.Data!.Identifier, Is.EqualTo("Test_ID"));
            Assert.That(result.Data!.Version.ToString(), Is.EqualTo("1.0.0"));

        }
    }
}
