using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Niconico.Download.Video.V3.DMS.HLS;
using Niconicome.Models.Domain.Niconico.Download.Video.V3.Error;
using Niconicome.Models.Domain.Utils.Error;
using Niconicome.Models.Helper.Result;

namespace Niconicome.Models.Domain.Niconico.Download.Video.V3.DMS
{
    public interface IStreamInfo
    {
        /// <summary>
        /// 垂直方向解像度
        /// </summary>
        int VerticalResolution { get; }

        /// <summary>
        /// Lowest
        /// </summary>
        bool IsLowest { get; }

        /// <summary>
        /// プレイリストのURL
        /// </summary>
        public string PlaylistURL { get; }

        /// <summary>
        /// 音声のURL
        /// </summary>
        public string AudioURL { get; }

        /// <summary>
        /// セグメントのURL
        /// </summary>
        IEnumerable<string> VideoSegmentURLs { get; }

        /// <summary>
        /// 音声セグメントのURL
        /// </summary>
        IEnumerable<string> AudioSegmentURLs { get; }

        /// <summary>
        /// 動画セグメントの長さ
        /// </summary>
        IEnumerable<SegmentDuration> VideoSegmentDurations { get; }

        /// <summary>
        /// 音声セグメントの長さ
        /// </summary>
        IEnumerable<SegmentDuration> AudioSegmentDurations { get; }

        /// <summary>
        /// キーのURL
        /// </summary>
        string VideoKeyURL { get; }

        /// <summary>
        /// IV
        /// </summary>
        string VideoIV { get; }

        /// <summary>
        /// 音声キーのURL
        /// </summary>
        string AudioKeyURL { get; }

        /// <summary>
        /// 音声のIV
        /// </summary>
        string AudioIV { get; }

        /// <summary>
        /// 初期化
        /// </summary>
        /// <param name="playlistURL"></param>
        /// <param name="audioURL"></param>
        /// <param name="verticalResolution"></param>
        void Initialize(string playlistURL, string audioURL, int verticalResolution, bool isLowest);

        /// <summary>
        /// 情報を取得
        /// </summary>
        Task<IAttemptResult> GetStreamInfo();
    }

    public record SegmentDuration(string Filename, float Duration);

    public class StreamInfo : IStreamInfo
    {
        public StreamInfo(INicoHttp httpHandler, IM3U8Parser m3U8Parser, IErrorHandler errorHandler)
        {
            this._httpHandler = httpHandler;
            this._m3U8Parser = m3U8Parser;
            this._errorHandler = errorHandler;
        }

        public string PlaylistURL { get; private set; } = string.Empty;

        public string AudioURL { get; private set; } = string.Empty;

        private readonly INicoHttp _httpHandler;

        private readonly IM3U8Parser _m3U8Parser;

        private readonly IErrorHandler _errorHandler;

        public int VerticalResolution { get; private set; }

        public bool IsLowest { get; private set; }

        public IEnumerable<string> VideoSegmentURLs { get; private set; } = new List<string>();

        public IEnumerable<string> AudioSegmentURLs { get; private set; } = new List<string>();

        public IEnumerable<SegmentDuration> VideoSegmentDurations { get; private set; } = new List<SegmentDuration>();

        public IEnumerable<SegmentDuration> AudioSegmentDurations { get; private set; } = new List<SegmentDuration>();

        public string VideoKeyURL { get; private set; } = string.Empty;

        public string VideoIV { get; private set; } = string.Empty;

        public string AudioKeyURL { get; private set; } = string.Empty;

        public string AudioIV { get; private set; } = string.Empty;

        public void Initialize(string playlistURL, string audioURL, int verticalResolution, bool isLowest)
        {
            this.PlaylistURL = playlistURL;
            this.AudioURL = audioURL;
            this.VerticalResolution = verticalResolution;
            this.IsLowest = isLowest;
        }


        public async Task<IAttemptResult> GetStreamInfo()
        {
            IAttemptResult<string> vResult = await this.GetContent(this.PlaylistURL);
            if (!vResult.IsSucceeded || vResult.Data is null)
            {
                return vResult;
            }

            IAttemptResult<string> aResult = await this.GetContent(this.AudioURL);
            if (!aResult.IsSucceeded || aResult.Data is null)
            {
                return aResult;
            }

            SegmentInfo video = this.Parse(vResult.Data);
            SegmentInfo audio = this.Parse(aResult.Data);

            this.AudioSegmentURLs = audio.segmentURL;
            this.VideoSegmentURLs = video.segmentURL;

            this.VideoSegmentDurations = new List<SegmentDuration>(video.segmentDurations.Select((duration, index) => new SegmentDuration(Path.GetFileName(new Uri(video.segmentURL[index]).AbsolutePath), duration)));
            this.AudioSegmentDurations = new List<SegmentDuration>(audio.segmentDurations.Select((duration, index) => new SegmentDuration(Path.GetFileName(new Uri(audio.segmentURL[index]).AbsolutePath), duration)));

            this.VideoKeyURL = video.keyURL;
            this.VideoIV = video.IV;
            this.AudioKeyURL = audio.keyURL;
            this.AudioIV = audio.IV;

            return AttemptResult.Succeeded();

        }

        private async Task<IAttemptResult<string>> GetContent(string url)
        {
            try
            {
                var res = await this._httpHandler.GetAsync(new Uri(url));
                if (!res.IsSuccessStatusCode)
                {
                    return AttemptResult<string>.Fail(this._errorHandler.HandleError(StreamParserError.FailedToGetPlaylistWithHttpError, (int)res.StatusCode, PlaylistURL));
                }

                return AttemptResult<string>.Succeeded(await res.Content.ReadAsStringAsync());

            }
            catch (Exception ex)
            {
                return AttemptResult<string>.Fail(this._errorHandler.HandleError(StreamParserError.FailedToGetPlaylist, ex, ex.Message));
            }
        }

        /// <summary>
        /// プレイリストを解析
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        private SegmentInfo Parse(string content)
        {
            IEnumerable<IM3U8Node> nodes = this._m3U8Parser.Parse(content);
            var streams = new List<string>();
            var durations = new List<float>();
            string key = string.Empty;
            string IV = string.Empty;

            foreach (var node in nodes)
            {
                if (node.Type == M3U8NodeType.Segment)
                {
                    streams.Add(node.URL);
                    if (float.TryParse(node.Value[0..^1], out float duration))
                    {
                        durations.Add(duration);
                    }
                    else
                    {
                        durations.Add(6);
                    }
                }
                else if (node.Type == M3U8NodeType.Key)
                {
                    var audioInfo = node.Value.Split(",");
                    var dict = new Dictionary<string, string>();
                    foreach (var info in audioInfo)
                    {
                        int equalIndex = info.IndexOf("=");
                        dict.Add(info[0..equalIndex], info[(equalIndex + 1)..]);
                    }

                    key = dict["URI"][1..^1];
                    IV = dict["IV"];
                }
            }

            return new SegmentInfo(streams.AsReadOnly(),durations.AsReadOnly(), key, IV);
        }

        private record SegmentInfo(IReadOnlyList<string> segmentURL,IReadOnlyList<float> segmentDurations, string keyURL, string IV);


    }
}
