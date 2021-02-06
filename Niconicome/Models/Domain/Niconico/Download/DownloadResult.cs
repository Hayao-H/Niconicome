using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niconicome.Models.Domain.Niconico.Download
{

    public interface IDownloadResult
    {
        bool Issucceeded { get; }
        string? Message { get; }
        string? VideoFileName { get; }
        uint VerticalResolution { get; }
    }

    /// <summary>
    /// 結果
    /// </summary>
    public class DownloadResult : IDownloadResult
    {
        public bool Issucceeded { get; set; }

        public string? Message { get; set; }

        public string? VideoFileName { get; set; }

        public uint VerticalResolution { get; set; }

    }
}
