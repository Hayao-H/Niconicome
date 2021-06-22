﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niconicome.Models.Domain.Niconico.Remote
{
    public class RemotePlaylistInfo
    {
        public List<VideoInfo> Videos { get; init; } = new();

        public string PlaylistName { get; set; } = string.Empty;
    }

    public class VideoInfo
    {
        public string ID { get; set; } = string.Empty;

        public string Title { get; set; } = string.Empty;

        public string UserName { get; set; } = string.Empty;

        public DateTime UploadedDT { get; set; } = DateTime.Now;

        public int UserID { get; set; }

        public long ViewCount { get; set; }

        public long CommentCount { get; set; }

        public long MylistCount { get; set; }

    }
}
