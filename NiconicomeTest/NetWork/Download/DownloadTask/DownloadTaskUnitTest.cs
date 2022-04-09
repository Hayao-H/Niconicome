using Microsoft.WindowsAPICodePack.Dialogs;
using Niconicome.Models.Network.Download;
using Niconicome.Models.Playlist;
using NUnit.Framework;
using Download = Niconicome.Models.Network.Download;

namespace NiconicomeTest.NetWork.Download.DownloadTask
{
    [TestFixture]
    class DownloadTaskUnitTest
    {
        [Test]
        public void 動画情報を変換する()
        {
            var video = new NonBindableListVideoInfo();
            video.Title.Value = "Hello World";
            video.NiconicoId.Value = "0";
            video.FileName.Value = "nyaa/a.mp4";
            video.IsEconomy.Value = true;

            var converted = new Download::DownloadTask(video, new DownloadSettings() { VerticalResolution = (uint)1080, FolderPath = @"fuga\hoge", PlaylistID = 1 });

            Assert.That(converted.Title, Is.EqualTo("Hello World"));
            Assert.That(converted.DirectoryPath, Is.EqualTo(@"fuga\hoge"));
            Assert.That(converted.PlaylistID, Is.EqualTo(1));
            Assert.That(converted.VerticalResolution, Is.EqualTo((uint)1080));
            Assert.That(converted.NiconicoID, Is.EqualTo("0"));
            Assert.That(converted.FilePath, Is.EqualTo(video.FileName.Value));
            Assert.That(converted.IsEconomyFile, Is.True);
        }

        [Test]
        public void キャンセルする()
        {
            var task = new Download::DownloadTask(new NonBindableListVideoInfo(), new DownloadSettings());
            task.Cancel();

            Assert.That(task.IsCanceled.Value, Is.True);
        }
    }
}
