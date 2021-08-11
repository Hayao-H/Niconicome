using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Playlist;

namespace Niconicome.Models.Network.Download
{
    public interface IDownloadResult
    {
        bool IsSucceeded { get; }
        bool IsCanceled { get; }
        string? Message { get; }
        string? VideoFileName { get; }
        IListVideoInfo VideoInfo { get; }
        uint VideoVerticalResolution { get; }
    }

    /// <summary>
    /// DL結果
    /// </summary>
    class DownloadResult : IDownloadResult
    {
        public bool IsSucceeded { get; set; }

        public bool IsCanceled { get; set; }

        public string? Message { get; set; }

        public string? VideoFileName { get; set; }

        public IListVideoInfo VideoInfo { get; set; } = new NonBindableListVideoInfo();

        public uint VideoVerticalResolution { get; set; }


    }
}
