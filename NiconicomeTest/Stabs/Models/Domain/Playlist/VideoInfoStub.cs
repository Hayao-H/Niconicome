using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Playlist;
using Niconicome.Models.Utils.Reactive;

namespace NiconicomeTest.Stabs.Models.Domain.Playlist
{
    internal class VideoInfoStub : IVideoInfo
    {
        public int SharedID { get; }

        public int ID { get; }

        public int PlaylistID { get; set; }

        public string NiconicoId { get; } = string.Empty;

        public string Title { get; set; } = string.Empty;

        public DateTime UploadedOn { get; set; }

        public DateTime AddedAt { get; set; }

        public int ViewCount { get; set; }

        public int CommentCount { get; set; }

        public int MylistCount { get; set; }

        public int LikeCount { get; set; }

        public string OwnerID { get; set; } = string.Empty;

        public string OwnerName { get; set; } = string.Empty;

        public string ChannelName { get; set; } = string.Empty;

        public string ChannelID { get; set; } = string.Empty;

        public string LargeThumbUrl { get; set; } = string.Empty;

        public string ThumbUrl { get; set; } = string.Empty;

        public IBindableProperty<string> ThumbPath { get; set; } = new BindableProperty<string>(string.Empty);

        public string FilePath { get; set; } = string.Empty;

        public IBindableProperty<string> Message { get; init; } = new BindableProperty<string>(string.Empty);

        public int Duration { get; set; }

        public IReadOnlyList<ITagInfo> Tags { get; } = new List<ITagInfo>();

        public bool IsDeleted { get; set; }

        public IBindableProperty<bool> IsSelected { get; } = new BindableProperty<bool>(false);

        public IBindableProperty<bool> IsDownloaded { get; } = new BindableProperty<bool>(false);

        public bool IsEconomy { get; set; }

        public bool IsAutoUpdateEnabled { get; set; }

        public void AddTag(ITagInfo tag)
        {

        }

        public void ClearTags()
        {

        }
    }
}
