using System.Collections.Generic;
using System.IO;
using System.Linq;
using Niconicome.Models.Domain.Local.IO.V2;
using Niconicome.Models.Domain.Niconico.Download.Video.V3.DMS;
using Niconicome.Models.Domain.Niconico.Net.Json;
using Niconicome.Models.Domain.Utils.Error;
using Niconicome.Models.Helper.Result;
using Err = Niconicome.Models.Domain.Niconico.Download.Video.V3.Error.StreamJsonHandlerError;
using Watch = Niconicome.Models.Domain.Local.Server.API.Watch.V1.LocalFile;

namespace Niconicome.Models.Domain.Niconico.Download.Video.V3.Local.StreamJson
{
    public interface IStreamJsonHandler : Watch::ILocalFileInfoHandler
    {
        IAttemptResult AddStream(string folderPath, int resolution, string videoKey, string audioKey, string videoIV, string audioIV, IEnumerable<SegmentDuration> videoSegments, IEnumerable<SegmentDuration> audioSegments, string videoMapFileName, string audioMapFileName, int bandWidth);
    }

    public class StreamJsonHandler : IStreamJsonHandler
    {
        public StreamJsonHandler(INiconicomeFileIO fileIO, IErrorHandler errorHandler)
        {
            this._fileIO = fileIO;
            this._errorHandler = errorHandler;
        }

        #region field

        private readonly INiconicomeFileIO _fileIO;

        private readonly IErrorHandler _errorHandler;

        #endregion

        public IAttemptResult AddStream(string folderPath, int resolution, string videoKey, string audioKey, string videoIV, string audioIV, IEnumerable<SegmentDuration> videoSegments, IEnumerable<SegmentDuration> audioSegments, string videoMapFileName, string audioMapFileName, int bandWidth)
        {
            var path = Path.Combine(folderPath, "stream.json");
            StreamType info;

            if (this._fileIO.Exists(path))
            {
                IAttemptResult<string> readResult = this._fileIO.Read(path);
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
            stream.AudioMapFileName = audioMapFileName;
            stream.VideoMapFileName = videoMapFileName;
            stream.VideoBandWidth = bandWidth;
            stream.VideoSegments = videoSegments.Select(s => new Segment() { Duration = s.Duration.ToString("N3"), FileName = s.Filename }).ToList();
            stream.AudioSegments = audioSegments.Select(s => new Segment() { Duration = s.Duration.ToString("N3"), FileName = s.Filename }).ToList();

            return this._fileIO.Write(path, JsonParser.Serialize(info));
        }

        public IAttemptResult<Watch::ILocalFileInfo> GetLocalFileInfo(string filePath)
        {
            if (!this._fileIO.Exists(filePath))
            {
                return AttemptResult<Watch::ILocalFileInfo>.Fail(this._errorHandler.HandleError(Err.FileNotExists));
            }

            IAttemptResult<string> readResult = this._fileIO.Read(filePath);
            if (!readResult.IsSucceeded || readResult.Data is null)
            {
                return AttemptResult<Watch::ILocalFileInfo>.Fail(readResult.Message);
            }

            var data = JsonParser.DeSerialize<StreamType>(readResult.Data);
            var info = new LocalFileInfo();

            foreach (var stream in data.Streams)
            {
                info.Streams.Add(stream.Resolution, new StreamInfo(stream));
            }

            return AttemptResult<Watch::ILocalFileInfo>.Succeeded(info);

        }
    }

    public class LocalFileInfo : Watch::ILocalFileInfo
    {
        public Dictionary<int, Watch::IStreamInfo> Streams { get; init; } = new();
    }

    public class StreamInfo : Watch::IStreamInfo
    {
        public StreamInfo(Stream stream)
        {
            this.Resolution = stream.Resolution;
            this.VideoKey = stream.VideoKey;
            this.AudioKey = stream.AudioKey;
            this.VideoIV = stream.VideoIV;
            this.AudioIV = stream.AudioIV;
            this.VideoMapFileName = stream.VideoMapFileName;
            this.AudioMapFileName = stream.AudioMapFileName;
            this.VideoBandWidth = stream.VideoBandWidth;
            this.VideoSegments = stream.VideoSegments;
            this.AudioSegments = stream.AudioSegments;
        }

        public int Resolution { get; init; }

        public string VideoKey { get; init; } 

        public string AudioKey { get; init; }

        public string VideoIV { get; init; }

        public string AudioIV { get; init; } 

        public string VideoMapFileName { get; init; } 

        public string AudioMapFileName { get; init; } 

        public int VideoBandWidth { get; init; }

        public IEnumerable<Watch::ISegmentInfo> VideoSegments { get; init; }

        public IEnumerable<Watch::ISegmentInfo> AudioSegments { get; init; }
    }
}
