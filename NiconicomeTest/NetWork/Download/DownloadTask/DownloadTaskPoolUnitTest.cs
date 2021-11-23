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
            var task = new Download::DownloadTask("", "", "", false, 1, new DownloadSettings());
            this.downloadTaskPool!.AddTask(task);

            Assert.That(this.downloadTaskPool!.Count, Is.EqualTo(1));
        }

        [Test]
        public void 複数のタスクを追加する()
        {
            var task1 = new Download::DownloadTask("", "", "", false, 1, new DownloadSettings());
            var task2 = new Download::DownloadTask("", "", "", false, 1, new DownloadSettings());
            var tasks = new List<Download::IDownloadTask>() { task1, task2 };
            this.downloadTaskPool!.AddTasks(tasks);

            Assert.That(this.downloadTaskPool!.Count, Is.EqualTo(2));
        }

        [Test]
        public void 追加イベントをチェックする()
        {
            int id = 0;
            var task = new Download::DownloadTask("", "", "", false, 1, new DownloadSettings());
            this.downloadTaskPool!.TaskPoolChange += (_, e) => id = e.Task.VideoID;
            this.downloadTaskPool!.AddTask(task);

            Assert.That(id, Is.EqualTo(task.VideoID));
        }

        [Test]
        public void タスクを削除する()
        {
            var task = new Download::DownloadTask("", "", "", false, 1, new DownloadSettings());
            this.downloadTaskPool!.AddTask(task);
            this.downloadTaskPool!.RemoveTask(task);

            Assert.That(this.downloadTaskPool!.Count, Is.EqualTo(0));
        }

        [Test]
        public void タスクの存在をチェックする()
        {
            var task = new Download::DownloadTask("", "", "", false, 1, new DownloadSettings());
            this.downloadTaskPool!.AddTask(task);

            var result = this.downloadTaskPool!.HasTask(t => t.ID == task.ID);

            Assert.That(result, Is.True);
        }
    }
}
