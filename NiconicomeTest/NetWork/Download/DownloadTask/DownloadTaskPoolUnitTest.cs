using System;
using NiconicomeTest.Stabs.Models.Local.State;
using NiconicomeTest.Stabs.Models.Network.Download;
using NiconicomeTest.Stabs.Models.Playlist.Playlist;
using NiconicomeTest.Stabs.Models.Playlist.VideoList;
using NUnit.Framework;
using Reactive.Bindings.Extensions;
using Download = Niconicome.Models.Network.Download.DLTask;

namespace NiconicomeTest.NetWork.Download.DownloadTask
{
    class DownloadTaskPoolUnitTest
    {
        private Download::IDownloadTaskPool? downloadTaskPool;

        [SetUp]
        public void SetUp()
        {
            this.downloadTaskPool = new Download::DownloadTaskPool();
        }

        [Test]
        public void タスクを追加する()
        {
            var task = new Download::DownloadTask(new PlaylistHandlerStub(), new MessageHandlerStub(), new ContentDownloadHelperStub(), new VideoListContainerStub(), new VideosUncheckerStub());
            this.downloadTaskPool!.AddTask(task);

            Assert.That(this.downloadTaskPool!.Tasks.Count, Is.EqualTo(1));
        }

        [Test]
        public void 追加イベントをチェックする()
        {
            bool isFired = true;
            this.downloadTaskPool!.Tasks.CollectionChangedAsObservable().Subscribe(e => isFired = true);
            var task = new Download::DownloadTask(new PlaylistHandlerStub(), new MessageHandlerStub(), new ContentDownloadHelperStub(), new VideoListContainerStub(), new VideosUncheckerStub());
            this.downloadTaskPool!.AddTask(task);

            Assert.That(isFired, Is.True);
        }

        [Test]
        public void タスクを削除する()
        {
            var task = new Download::DownloadTask(new PlaylistHandlerStub(), new MessageHandlerStub(), new ContentDownloadHelperStub(), new VideoListContainerStub(), new VideosUncheckerStub());
            this.downloadTaskPool!.AddTask(task);
            this.downloadTaskPool!.RemoveTask(task);

            Assert.That(this.downloadTaskPool!.Tasks.Count, Is.EqualTo(0));
        }

        [Test]
        public void 複数のタスクを削除する()
        {
            var task1 = new Download::DownloadTask(new PlaylistHandlerStub(), new MessageHandlerStub(), new ContentDownloadHelperStub(), new VideoListContainerStub(), new VideosUncheckerStub());
            var task2 = new Download::DownloadTask(new PlaylistHandlerStub(), new MessageHandlerStub(), new ContentDownloadHelperStub(), new VideoListContainerStub(), new VideosUncheckerStub());
            var task3 = new Download::DownloadTask(new PlaylistHandlerStub(), new MessageHandlerStub(), new ContentDownloadHelperStub(), new VideoListContainerStub(), new VideosUncheckerStub());

            foreach (var task in new[] { task1, task2, task3 })
            {
                this.downloadTaskPool!.AddTask(task);
            }

            foreach (var task in new[] { task1, task2, task3 })
            {
                this.downloadTaskPool!.RemoveTask(task);
            }

            Assert.That(this.downloadTaskPool!.Tasks.Count, Is.Zero);
        }

        [Test]
        public void タスクをクリア()
        {
            var task1 = new Download::DownloadTask(new PlaylistHandlerStub(), new MessageHandlerStub(), new ContentDownloadHelperStub(), new VideoListContainerStub(), new VideosUncheckerStub());
            var task2 = new Download::DownloadTask(new PlaylistHandlerStub(), new MessageHandlerStub(), new ContentDownloadHelperStub(), new VideoListContainerStub(), new VideosUncheckerStub());
            var task3 = new Download::DownloadTask(new PlaylistHandlerStub(), new MessageHandlerStub(), new ContentDownloadHelperStub(), new VideoListContainerStub(), new VideosUncheckerStub());

            foreach (var task in new[] { task1, task2, task3 })
            {
                this.downloadTaskPool!.AddTask(task);
            }

            this.downloadTaskPool!.Clear();

            Assert.That(this.downloadTaskPool!.Tasks.Count, Is.Zero);
        }

        [Test]
        public void 完了済みフィルターをテスト()
        {
            this.downloadTaskPool!.DisplayCompleted.Value = false;

            var task1 = new Download::DownloadTask(new PlaylistHandlerStub(), new MessageHandlerStub(), new ContentDownloadHelperStub(), new VideoListContainerStub(), new VideosUncheckerStub());
            var task2 = new Download::DownloadTask(new PlaylistHandlerStub(), new MessageHandlerStub(), new ContentDownloadHelperStub(), new VideoListContainerStub(), new VideosUncheckerStub());
            var task3 = new Download::DownloadTask(new PlaylistHandlerStub(), new MessageHandlerStub(), new ContentDownloadHelperStub(), new VideoListContainerStub(), new VideosUncheckerStub());

            task1.IsCompleted.Value = true;
            task2.IsCompleted.Value = true;

            foreach (var task in new[] { task1, task2, task3 })
            {
                this.downloadTaskPool!.AddTask(task);
            }


            Assert.That(this.downloadTaskPool!.Tasks.Count, Is.EqualTo(1));
        }

        [Test]
        public void キャンセル済みフィルターをテスト()
        {
            this.downloadTaskPool!.DisplayCanceled.Value = false;

            var task1 = new Download::DownloadTask(new PlaylistHandlerStub(), new MessageHandlerStub(), new ContentDownloadHelperStub(), new VideoListContainerStub(), new VideosUncheckerStub());
            var task2 = new Download::DownloadTask(new PlaylistHandlerStub(), new MessageHandlerStub(), new ContentDownloadHelperStub(), new VideoListContainerStub(), new VideosUncheckerStub());
            var task3 = new Download::DownloadTask(new PlaylistHandlerStub(), new MessageHandlerStub(), new ContentDownloadHelperStub(), new VideoListContainerStub(), new VideosUncheckerStub());

            task1.IsCanceled.Value = true;
            task3.IsCanceled.Value = true;

            foreach (var task in new[] { task1, task2, task3 })
            {
                this.downloadTaskPool!.AddTask(task);
            }


            Assert.That(this.downloadTaskPool!.Tasks.Count, Is.EqualTo(1));
        }


    }
}
