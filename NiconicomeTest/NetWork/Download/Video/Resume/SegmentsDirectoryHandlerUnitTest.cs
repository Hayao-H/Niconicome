using System.Collections.Generic;
using Niconicome.Models.Domain.Local.IO;
using Niconicome.Models.Domain.Niconico.Download.Video.Resume;
using NiconicomeTest.Stabs.Models.Domain.Local.IO;
using NUnit.Framework;

namespace NiconicomeTest.NetWork.Download.Video.Resume
{
    [TestFixture]
    class SegmentsDirectoryHandlerUnitTest
    {
        private INicoDirectoryIO? nicoDirectoryIO;

        private ISegmentsDirectoryHandler? segmentsDirectoryHandler;

        private IStreamResumer? streamResumer;

        [Test]
        public void 存在しないディレクトリをチェックする()
        {
            this.nicoDirectoryIO = new NicoDirectoryIOMock(_ => false, (_, _, _) => new List<string>(), (_, _, _) => new List<string>());
            this.segmentsDirectoryHandler = new SegmentsDirectoryHandler(this.nicoDirectoryIO);
            this.streamResumer = new StreamResumer(this.nicoDirectoryIO, this.segmentsDirectoryHandler);

            var result = this.streamResumer.SegmentsDirectoryExists("sm9");

            Assert.That(result, Is.False);
        }

        [Test]
        public void 不正なディレクトリをチェックする()
        {
            this.nicoDirectoryIO = new NicoDirectoryIOMock(_ => false, (_, _, _) => new List<string>(), (_, _, _) => new List<string>() { "sm9-1080-2021-04-09-20" });
            this.segmentsDirectoryHandler = new SegmentsDirectoryHandler(this.nicoDirectoryIO);
            this.streamResumer = new StreamResumer(this.nicoDirectoryIO, this.segmentsDirectoryHandler);

            var result = this.streamResumer.SegmentsDirectoryExists("sm9");

            Assert.That(result, Is.False);
        }

        [Test]
        public void 存在するディレクトリをチェックする()
        {
            this.nicoDirectoryIO = new NicoDirectoryIOMock(_ => false, (_, _, _) => new List<string>(), (_, _, _) => new List<string>() { "sm9-1080-2021-04-09" });
            this.segmentsDirectoryHandler = new SegmentsDirectoryHandler(this.nicoDirectoryIO);
            this.streamResumer = new StreamResumer(this.nicoDirectoryIO, this.segmentsDirectoryHandler);

            var result = this.streamResumer.SegmentsDirectoryExists("sm9");

            Assert.That(result, Is.True);
        }
    }
}
