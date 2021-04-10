using System;
using System.Collections.Generic;
using System.IO;
using Niconicome.Models.Domain.Local.IO;
using Niconicome.Models.Domain.Niconico.Download.Video.Resume;
using NiconicomeTest.Stabs.Models.Domain.Local.IO;
using NUnit.Framework;

namespace NiconicomeTest.NetWork.Download.Video.Resume
{

    [TestFixture]
    class SegmentDirectoryHandlerUnitTest
    {
        private INicoDirectoryIO? nicoDirectoryIO;

        private ISegmentsDirectoryHandler? segmentsDirectoryHandler;

        [Test]
        public void 存在しないディレクトリを渡す()
        {
            this.nicoDirectoryIO = new NicoDirectoryIOMock(_ => true, (_, _, _) => new List<string>(), (_, _, _) => new List<string>());
            this.segmentsDirectoryHandler = new SegmentsDirectoryHandler(this.nicoDirectoryIO);

            Assert.That(() => this.segmentsDirectoryHandler.GetSegmentsDirectoryInfo("sm9"), Throws.InstanceOf<IOException>());
        }

        [Test]
        public void 不正なパスを渡す()
        {
            this.nicoDirectoryIO = new NicoDirectoryIOMock(_ => true, (_, _, _) => new List<string>(), (_, _, _) => new List<string>() { @"tmp\sm9-hoge-2021-04-09" });
            this.segmentsDirectoryHandler = new SegmentsDirectoryHandler(this.nicoDirectoryIO);

            Assert.That(() => this.segmentsDirectoryHandler.GetSegmentsDirectoryInfo("sm9"), Throws.InstanceOf<InvalidOperationException>());
        }

        [Test]
        public void セグメント0個()
        {
            this.nicoDirectoryIO = new NicoDirectoryIOMock(_ => true, (_, _, _) => new List<string>(), (_, _, _) => new List<string>() { @"tmp\sm9-1080-2021-04-09" });
            this.segmentsDirectoryHandler = new SegmentsDirectoryHandler(this.nicoDirectoryIO);
            var result = this.segmentsDirectoryHandler.GetSegmentsDirectoryInfo("sm9");

            Assert.That(result.DirectoryName, Is.EqualTo("sm9-1080-2021-04-09"));
            Assert.That(result.ExistsFileNames.Count, Is.EqualTo(0));
            Assert.That(result.Resolution, Is.EqualTo((uint)1080));
            Assert.That(result.NiconicoID, Is.EqualTo("sm9"));

        }

        [Test]
        public void セグメント3個()
        {
            this.nicoDirectoryIO = new NicoDirectoryIOMock(_ => true, (_, _, _) => new List<string>() { "1.ts", "2.ts", "3.ts", "combined.ts" }, (_, _, _) => new List<string>() { @"tmp\sm9-1080-2021-04-09" });
            this.segmentsDirectoryHandler = new SegmentsDirectoryHandler(this.nicoDirectoryIO);
            var result = this.segmentsDirectoryHandler.GetSegmentsDirectoryInfo("sm9");

            Assert.That(result.DirectoryName, Is.EqualTo("sm9-1080-2021-04-09"));
            Assert.That(result.ExistsFileNames.Count, Is.EqualTo(3));
            Assert.That(result.Resolution, Is.EqualTo((uint)1080));
            Assert.That(result.NiconicoID, Is.EqualTo("sm9"));

        }
    }
}
