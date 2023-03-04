using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Local;
using Niconicome.Models.Domain.Local.Playlist;
using Niconicome.Models.Infrastructure.Database.LiteDB;

namespace Niconicome.Models.Infrastructure.Database.Types
{
    public class Playlist : IBaseStoreClass
    {
        public int Id { get; set; }

        public string TableName { get; init; } = TableNames.Playlist;

        public string Name { get; set; } = string.Empty;

        public string FolderPath { get; set; } = string.Empty;

        public string RemoteParameter { get; set; } = string.Empty;

        public DBPlaylistType PlaylistType { get; set; }

        public DBSortType SortType { get; set; }

        public bool IsAscendant { get; set; }

        public List<string> Videos { get; set; } = new();

        public List<int> Children { get; set; } = new();

    }
}
