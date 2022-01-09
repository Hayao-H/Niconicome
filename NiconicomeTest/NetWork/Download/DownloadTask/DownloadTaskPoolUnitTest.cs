using System.Collections.Generic;
using Niconicome.Models.Network.Download;
using Niconicome.Models.Playlist;
using NUnit.Framework;
using Download = Niconicome.Models.Network.Download;

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
            var task = new Download::DownloadTask(new NonBindableListVideoInfo(), new DownloadSettings());
            this.downloadTaskPool!.AddTask(task);

            Assert.That(this.downloadTaskPool!.Tasks.Count, Is.EqualTo(1));
        }

        [Test]
        public void 複数のタスクを追加する()
        {
            var task1 = new Download::DownloadTask(new NonBindableListVideoInfo(), new DownloadSettings());
            var task2 = new Download::DownloadTask(new NonBindableListVideoInfo(), new DownloadSettings());
            var tasks = new List<Download::IDownloadTask>() { task1, task2 };
            this.downloadTaskPool!.AddTasks(tasks);

            Assert.That(this.downloadTaskPool!.Tasks.Count, Is.EqualTo(2));
        }

        [Test]
        public void 追加イベントをチェックする()
        {
            var id = "";
            this.downloadTaskPool!.RegisterAddHandler(task => id = task.NiconicoID);
            var task = new Download::DownloadTask(new NonBindableListVideoInfo() { NiconicoId = new Reactive.Bindings.ReactiveProperty<string>("sm9") }, new DownloadSettings());
            this.downloadTaskPool!.AddTask(task);

            Assert.That(id, Is.EqualTo(task.NiconicoID));
        }

        [Test]
        public void タスクを削除する()
        {
            var task = new Download::DownloadTask(new NonBindableListVideoInfo(), new DownloadSettings());
            this.downloadTaskPool!.AddTask(task);
            this.downloadTaskPool!.RemoveTask(task);

            Assert.That(this.downloadTaskPool!.Tasks.Count, Is.EqualTo(0));
        }

        [Test]
        public void 複数のタスクを削除する()
        {
            var task1 = new Download::DownloadTask(new NonBindableListVideoInfo(), new DownloadSettings());
            var task2 = new Download::DownloadTask(new NonBindableListVideoInfo(), new DownloadSettings());
            var task3 = new Download::DownloadTask(new NonBindableListVideoInfo(), new DownloadSettings());
            this.downloadTaskPool!.AddTasks(new[] { task1, task2, task3 });
            this.downloadTaskPool!.RemoveTasks(new[] { task1, task2, task3 });

            Assert.That(this.downloadTaskPool!.Tasks.Count, Is.Zero);
        }

        [Test]
        public void 条件に合致するタスクを削除()
        {
            var task = new Download::DownloadTask(new NonBindableListVideoInfo() { NiconicoId = new Reactive.Bindings.ReactiveProperty<string>("sm9") }, new DownloadSettings());
            this.downloadTaskPool!.AddTask(task);
            this.downloadTaskPool.RemoveTasks(task => task.NiconicoID == "sm9");


            Assert.That(this.downloadTaskPool!.Tasks.Count, Is.Zero);
        }

        [Test]
        public void タスクをクリア()
        {
            var task1 = new Download::DownloadTask(new NonBindableListVideoInfo(), new DownloadSettings());
            var task2 = new Download::DownloadTask(new NonBindableListVideoInfo(), new DownloadSettings());
            var task3 = new Download::DownloadTask(new NonBindableListVideoInfo(), new DownloadSettings());
            this.downloadTaskPool!.AddTasks(new[] { task1, task2, task3 });
            this.downloadTaskPool!.Clear();

            Assert.That(this.downloadTaskPool!.Tasks.Count, Is.Zero);
        }

        public void フィルター関数をテスト()
        {
            var task1 = new Download::DownloadTask(new NonBindableListVideoInfo() { NiconicoId = new Reactive.Bindings.ReactiveProperty<string>("sm9") }, new DownloadSettings());
            var task2 = new Download::DownloadTask(new NonBindableListVideoInfo(), new DownloadSettings());
            var task3 = new Download::DownloadTask(new NonBindableListVideoInfo(), new DownloadSettings());
            this.downloadTaskPool!.AddTasks(new[] { task1, task2, task3 });

            this.downloadTaskPool!.RegisterFilter(task => task.NiconicoID == "sm9");
            this.downloadTaskPool!.Refresh();

            Assert.That(this.downloadTaskPool!.Tasks.Count, Is.EqualTo(1));
        }

        [Test]
        public void タスクをキャンセル()
        {
            var task1 = new Download::DownloadTask(new NonBindableListVideoInfo(), new DownloadSettings());
            var task2 = new Download::DownloadTask(new NonBindableListVideoInfo(), new DownloadSettings());
            var task3 = new Download::DownloadTask(new NonBindableListVideoInfo(), new DownloadSettings());
            this.downloadTaskPool!.AddTasks(new[] { task1, task2, task3 });
            this.downloadTaskPool!.CancelAllTasks();

            Assert.That(task1.IsCanceled.Value, Is.True);
            Assert.That(task2.IsCanceled.Value, Is.True);
            Assert.That(task2.IsCanceled.Value, Is.True);
            Assert.That(this.downloadTaskPool!.Tasks.Count, Is.Zero);
        }

    }
}
