using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niconicome.Models.Domain.Local.External.Software.FFmpeg.ffprobe
{
    public interface IFFprobeResult
    {
        public int Height { get; }
    }

    public record FFprobeResult(int Height) : IFFprobeResult;
}
