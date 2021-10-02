using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Const;
using Niconicome.Models.Domain.Local.Addons.API.Resource;
using Niconicome.Models.Domain.Local.Addons.Core.Engine;
using Niconicome.Models.Helper.Result;
using NiconicomeTest.Stabs.Models.Domain.Local.IO;
using NiconicomeTest.Stabs.Models.Domain.Utils;
using NUnit.Framework;

namespace NiconicomeTest.Local.Addons.API.Resource
{
    [TestFixture]
    internal class ResourceHandlerUnitTest
    {
        private NicoFileIOMock? _nicoFileIOMock;

        private IResourceHander? _handler;

        [SetUp]
        public void SetUp()
        {
            this._nicoFileIOMock = new NicoFileIOMock(() => true, () => "resource");
            this._handler = new ResourceHander(this._nicoFileIOMock, new AddonLogger(new LoggerStab()));
        }

        [Test]
        public void 未初期化状態で読み込む()
        {
            Assert.That(() => this._handler!.GetResource(string.Empty), Throws.InvalidOperationException);
        }

        [Test]
        public void ファイルを読み込む()
        {
            var path = "path/to/the/resource";
            this._handler!.Initialize("id", "");
            IAttemptResult<string> result = this._handler!.GetResource(path);

            Assert.That(result.IsSucceeded, Is.True);
            Assert.That(result.Data, Is.EqualTo("resource"));
            Assert.That(this._nicoFileIOMock!.LastOpendFilePath, Is.EqualTo(Path.Combine(FileFolder.AddonsFolder, "id", AdddonConstant.ResourceDirectoryName, path)));
        }
    }
}
