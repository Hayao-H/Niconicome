using System;
using System.Collections.Generic;

namespace Niconicome.Models.Domain.Niconico.Remote.Series
{
    public class SeriesInfo
    {
        public string SeriesName { get; set; } = string.Empty;

        public List<SeriesVideoInfo> SeriesVideos { get; set; } = new();
    }

    public class SeriesVideoInfo
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
