using NUnit.Framework;
using Niconicome.Models.Domain.Utils;
using Niconicome.Models.Domain.Local.Store;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using System.Linq;
using NiconicomeTest.Stabs.Models.Domain.Utils;

namespace NiconicomeTest.Local
{
    [TestFixture]
    class VideoFileUnitTest
    {
        private IVideoFileStorehandler? handler;

        [SetUp]
        public void SetUp()
        {
            Static.DataBaseInstance.Clear(Niconicome.Models.Domain.Local.Store.Types.VideoFile.TableName);
            this.handler = new VideoFileStorehandler(Static.DataBaseInstance, new LoggerStub());
        }

        [Test]
        public void ファイルパスを保存して確認する()
        {
            string assemblyPath = Assembly.GetExecutingAssembly().Location;
            this.handler!.Add("0", assemblyPath);

            Assert.IsTrue(this.handler.Exists("0"));
        }

        [Test]
        public void ファイルパスを削除して確認する()
        {
            string assemblyPath = Assembly.GetExecutingAssembly().Location;
            this.handler!.Add("0", assemblyPath);
            this.handler!.Delete("0", assemblyPath);

            Assert.IsFalse(this.handler.Exists("0"));
        }

        [Test]
        public void 存在しないパスを保存して確認する()
        {
            string invalidPath = @"Z:\this\is\invalid\path";
            this.handler!.Add("0", invalidPath);

            Assert.IsFalse(this.handler!.Exists("0"));
            Assert.AreEqual(0, this.handler!.GetFilePaths("0").Count());
        }
    }
}
