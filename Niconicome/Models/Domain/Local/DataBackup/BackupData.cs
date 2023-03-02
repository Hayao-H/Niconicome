using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niconicome.Models.Domain.Local.DataBackup
{
    public interface IBackupData
    {
        string Name { get; }
        string GUID { get; }
        string Filename { get; }
        DateTime CreatedOn { get; }
        float FileSize { get; }
    }

    public class BackupData : IBackupData
    {
        public BackupData(string name)
        {
            this.GUID = Guid.NewGuid().ToString("D");
            this.Name = name;
            this.Filename = $"{this.GUID}.dbbackup";
            this.CreatedOn = DateTime.Now;
        }

        public string Name { get; init; }

        public string GUID { get; init; }

        public string Filename { get; init; }

        public DateTime CreatedOn { get; init; }

        public float FileSize { get; set; }
    }
}
