using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Local.IO.V2;
using Niconicome.Models.Domain.Niconico.Download.Video.V3.DMS;
using Niconicome.Models.Domain.Niconico.Net.Json;
using Niconicome.Models.Helper.Result;

namespace Niconicome.Models.Domain.Niconico.Download.Video.V3.Local.StreamJson
{
    public interface IStreamJsonHandler
    {
        IAttemptResult AddStream(string folderPath, int resolution, string videoKey, string audioKey, string videoIV, string audioIV, IEnumerable<SegmentDuration> videoSegments, IEnumerable<SegmentDuration> audioSegments);
    }

    public class StreamJsonHandler : IStreamJsonHandler
    {
        public StreamJsonHandler(INiconicomeFileIO fileIO)
        {
            this.fileIO = fileIO;
        }

        #region field

        private readonly INiconicomeFileIO fileIO;

        #endregion

        public IAttemptResult AddStream(string folderPath, int resolution, string videoKey, string audioKey, string videoIV, string audioIV, IEnumerable<SegmentDuration> videoSegments, IEnumerable<SegmentDuration> audioSegments)
        {
            var path = Path.Combine(folderPath, "stream.json");
            StreamType info;

            if (this.fileIO.Exists(path))
            {
                IAttemptResult<string> readResult = this.fileIO.Read(path);
                if (!readResult.IsSucceeded || readResult.Data is null)
                {
                    return readResult;
                }

                info = JsonParser.DeSerialize<StreamType>(readResult.Data);
            }
            else
            {
                info = new StreamType();
            }

            Stream stream;

            if (info.Streams.Any(s => s.Resolution == resolution))
            {
                stream = info.Streams.First(s => s.Resolution == resolution);
            }
            else
            {
                stream = new Stream();
                info.Streams.Add(stream);
            }

            stream.Resolution = resolution;
            stream.VideoKey = videoKey;
            stream.AudioKey = audioKey;
            stream.VideoIV = videoIV;
            stream.AudioIV = audioIV;
            stream.VideoSegments = videoSegments.Select(s => new Segment() { Duration = s.Duration.ToString("N3"), FileName = s.Filename }).ToList();
            stream.AudioSegments = audioSegments.Select(s => new Segment() { Duration = s.Duration.ToString("N3"), FileName = s.Filename }).ToList();

            return this.fileIO.Write(path, JsonParser.Serialize(info));
        }
    }
}
