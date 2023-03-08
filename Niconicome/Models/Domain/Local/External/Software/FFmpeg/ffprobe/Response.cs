using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Niconicome.Models.Domain.Local.External.Software.FFmpeg.ffprobe
{
    public class Response
    {
        [JsonPropertyName("streams")]
        public List<Stream> Streams { get; set; } = new();
    }

    public class Stream
    {

        [JsonPropertyName("codec_type")]
        public string CodecType { get; set; } = string.Empty;

        [JsonPropertyName("height")]
        public int Height { get; set; }
    }

}
