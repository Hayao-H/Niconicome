using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteDB;
using Niconicome.Models.Infrastructure.Database.LiteDB;

namespace Niconicome.Models.Infrastructure.Database.Types
{
    public class VideoFile : IBaseStoreClass
    {
        public int Id { get; set; }

        [BsonIgnore]
        public string TableName { get; init; } = TableNames.VideoFile;

        public string DirectoryPath { get; set; } = string.Empty;

        public string NiconicoID { get; set; } = string.Empty;

        public string FilePath { get; set; } = string.Empty;

        public uint VerticalResolution { get; set; }
    }
}
