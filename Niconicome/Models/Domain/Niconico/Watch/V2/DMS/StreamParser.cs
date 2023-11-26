using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Niconico.Watch.V2.DMS.Error;
using Niconicome.Models.Domain.Niconico.Watch.V2.DMS.HLS;
using Niconicome.Models.Domain.Utils;
using Error = Niconicome.Models.Domain.Utils.Error;
using Niconicome.Models.Helper.Result;

namespace Niconicome.Models.Domain.Niconico.Watch.V2.DMS
{
    public interface IStreamParser
    {
        /// <summary>
        /// master.m3u8をパースする
        /// 子プレイリストはmaster.m3u8のURLから取得する
        /// </summary>
        /// <param name="m3u8"></param>
        /// <returns></returns>
        Task<IAttemptResult<IStreamCollection>> ParseAsync(string masterURL);
    }

    public class StreamParser : IStreamParser
    {
        public StreamParser(INicoHttp httpHandler, Error::IErrorHandler errorHandler, IM3U8Parser m3U8Parser)
        {
            this._httpHandler = httpHandler;
            this._errorHandler = errorHandler;
            this._m3U8Parser = m3U8Parser;
        }


        private readonly INicoHttp _httpHandler;

        private readonly Error::IErrorHandler _errorHandler;

        private readonly IM3U8Parser _m3U8Parser;

        public async Task<IAttemptResult<IStreamCollection>> ParseAsync(string masterURL)
        {
            string content;

            try
            {
                var res = await this._httpHandler.GetAsync(new Uri(masterURL));
                if (!res.IsSuccessStatusCode)
                {
                    return AttemptResult<IStreamCollection>.Fail(this._errorHandler.HandleError(StreamParserError.FailedToGetPlaylistWithHttpError, (int)res.StatusCode, masterURL));
                }

                content = await res.Content.ReadAsStringAsync();

            }
            catch (Exception ex)
            {
                return AttemptResult<IStreamCollection>.Fail(this._errorHandler.HandleError(StreamParserError.FailedToGetPlaylist, ex, ex.Message));
            }

            IEnumerable<IM3U8Node> nodes = this._m3U8Parser.Parse(content);
            var streams = new List<IStreamInfo>();
            var audios = new Dictionary<string, string>();

            foreach (var node in nodes)
            {
                if (node.Type == M3U8NodeType.StreamInfo)
                {
                    var streamInfo = node.Value.Split(",");
                    var dict = new Dictionary<string, string>();
                    foreach (var info in streamInfo)
                    {
                        var kv = info.Split("=");
                        if (kv.Length != 2) continue;
                        dict.Add(kv[0], kv[1]);
                    }

                    var resolution = this.GetVerticalResolution(dict["RESOLUTION"]);
                    var playlistURL = node.URL;
                    var stream = DIFactory.Resolve<IStreamInfo>();
                    stream.Initialize(playlistURL, audios[dict["AUDIO"]], resolution,playlistURL.Contains("lowest"));
                    streams.Add(stream);
                }
                else if (node.Type == M3U8NodeType.Audio)
                {
                    var streamInfo = node.Value.Split(",");
                    var dict = new Dictionary<string, string>();
                    foreach (var info in streamInfo)
                    {
                        var kv = info.Split("=");
                        dict.Add(kv[0], kv[1]);
                    }

                    audios.Add(dict["GROUP-ID"], dict["URI"][1..^1]);

                }
            }

            return AttemptResult<IStreamCollection>.Succeeded(new StreamCollection(streams));

        }

        /// <summary>
        /// 垂直方向の解像度をuintで取得する関数
        /// </summary>
        /// <param name="resolution"></param>
        /// <returns></returns>
        private int GetVerticalResolution(string resolution)
        {
            var res = resolution.Split("x");
            return int.Parse(res[1]);
        }

    }
}
