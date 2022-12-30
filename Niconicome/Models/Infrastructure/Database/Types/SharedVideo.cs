using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Infrastructure.Database.LiteDB;

namespace Niconicome.Models.Infrastructure.Database.Types
{
    public class SharedVideo : IBaseStoreClass
    {
        public int Id { get; set; }

        public string TableName { get; set; } = TableNames.SharedVideo;

        public string NiconicoId { get; set; } = string.Empty;

        public string Title { get; set; } = string.Empty;

        public DateTime UploadedOn { get; set; }

        public int ViewCount { get; set; }

        public int CommentCount { get; set; }

        public int MylistCount { get; set; }

        public int LikeCount { get; set; }

        public int OwnerID { get; set; }

        public string OwnerName { get; set; } = string.Empty;

        public string LargeThumbUrl { get; set; } = string.Empty;

        public string ThumbUrl { get; set; } = string.Empty;

        public string ThumbPath { get; set; } = string.Empty;

        public int Duration { get; set; }

        public List<int> Tags { get; set; } = new();

        public bool IsDeleted { get; set; }
    }
}
