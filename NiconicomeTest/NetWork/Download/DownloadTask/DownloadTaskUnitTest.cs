using System;
using System.Collections.Generic;
using Niconicome.Models.Local.State;
using Niconicome.Models.Network.Download;
using Niconicome.Models.Playlist;
using NiconicomeTest.Stabs.Models.Local.State;
using NiconicomeTest.Stabs.Models.Network.Download;
using NiconicomeTest.Stabs.Models.Playlist.Playlist;
using NiconicomeTest.Stabs.Models.Playlist.VideoList;
using NUnit.Framework;
using Reactive.Bindings.Extensions;
using Download = Niconicome.Models.Network.Download.DLTask;

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

            var task = new Download::DownloadTask(new PlaylistHandlerStub(), new MessageHandlerStub(), new ContentDownloadHelperStub(), new VideoListContainerStub(), new VideosUncheckerStub());

            task.Initialize(video, new DownloadSettings() { PlaylistID = 1 });

            Assert.That(task.Title, Is.EqualTo("Hello World"));
            Assert.That(task.PlaylistID, Is.EqualTo(1));
            Assert.That(task.NiconicoID, Is.EqualTo("0"));
        }

        [Test]
        public void キャンセルする()
        {
            var task = new Download::DownloadTask(new PlaylistHandlerStub(), new MessageHandlerStub(), new ContentDownloadHelperStub(), new VideoListContainerStub(), new VideosUncheckerStub());

            task.Cancel();

            Assert.That(task.IsCanceled.Value, Is.True);
        }
    }
}
