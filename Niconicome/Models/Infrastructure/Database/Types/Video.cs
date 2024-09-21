using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Infrastructure.Database.LiteDB;

namespace Niconicome.Models.Infrastructure.Database.Types
{
    public class Video : IBaseStoreClass
    {
        public int Id { get; set; }

        public string TableName { get; set; } = TableNames.Video;

        public int SharedVideoID { get; set; }

        public int PlaylistID { get; set; }

        public bool IsSelected { get; set; }

        public bool IsDownloaded { get; set; }

        public bool IsEconomy { get; set; }

        public DateTime AddedAt { get; set; }
    }
}
