using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Niconico.Download.Video.V3.Local.DMS;
using NiconicomeTest.Stabs.Models.Domain.Local.IO.V2;
using NUnit.Framework;

namespace NiconicomeTest.NetWork.Download.Video.V2.Local.DMS
{


    [TestFixture]
    public class DMSFileHandlerTests
    {
        private IDMSFileHandler? _dmsFileHandler;

        [Test]
        public void ディレクトリが存在する場合()
        {
            _dmsFileHandler = new DMSFileHandler(new NiconicomeDirectoryIOStub(new List<string>(), new List<string>() { Path.Combine(@"C:\", "video", "1080") }));

            var result = this._dmsFileHandler.Exists("id", "dirPath");

            Assert.That(result, Is.True);
        }

        [Test]
        public void ディレクトリが存在しない場合()
        {
            _dmsFileHandler = new DMSFileHandler(new NiconicomeDirectoryIOStub(new List<string>(), new List<string>()));

            var result = this._dmsFileHandler.Exists("id", "dirPath");

            Assert.That(result, Is.False);
        }

        [Test]
        public void エコノミーである場合()
        {
            var p1 = Path.Combine(@"C:\", "video", "360");
            var p2 = Path.Combine(@"C:\", "video", "144");
            _dmsFileHandler = new DMSFileHandler(new NiconicomeDirectoryIOStub(new List<string>(), new List<string>() { p1, p2 }));

            var result = this._dmsFileHandler.IsEconomy("id", "dirPath");

            Assert.That(result, Is.True);
        }

        [Test]
        public void エコノミーでない場合()
        {
            var p1 = Path.Combine(@"C:\", "video", "360");
            var p2 = Path.Combine(@"C:\", "video", "1080");
            _dmsFileHandler = new DMSFileHandler(new NiconicomeDirectoryIOStub(new List<string>(), new List<string>() { p1, p2 }));

            var result = this._dmsFileHandler.IsEconomy("id", "dirPath");

            Assert.That(result, Is.False);
        }
    }

}
