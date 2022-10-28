using NUnit.Framework;
using System;
using Niconicome.Models.Local;
using Niconicome.Models.Domain.Utils;
using System.IO;
using System.Linq;
using NiconicomeTest.Stabs.Models.Domain.Utils;

namespace NiconicomeTest.Local.Backup
{
    [TestFixture]
    class BackupUnitTest
    {
        private IBackuphandler? backuphandler;

        [SetUp]
        public void Setup()
        {
            this.backuphandler = new BackupHandler(new LoggerStub(),Static.DataBaseInstance);
            string path = Path.Combine(AppContext.BaseDirectory, "backups");
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }
        }

        [Test]
        public void バックアップを作成する()
        {
            this.backuphandler!.CreateBackup("test","test.db");

            Assert.AreEqual(1, this.backuphandler!.GetAllBackups().Count());
        }
    }

}
