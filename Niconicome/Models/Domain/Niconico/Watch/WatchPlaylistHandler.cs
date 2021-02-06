using System;
using System.Net.Http;
using System.Threading.Tasks;
using Niconicome.Extensions.System;
using Dmc = Niconicome.Models.Domain.Niconico.Dmc;

namespace Niconicome.Models.Domain.Niconico.Watch
{

    public interface IWatchPlaylisthandler
    {
        Task<Dmc::IStreamInfo> GetStreamInfoAsync(string playlistUrl);
    }

    class WatchPlaylistHandler : IWatchPlaylisthandler
    {

        public WatchPlaylistHandler(Dmc::IStreamhandler handler, INicoHttp http)
        {
            this.handler = handler;
            this.http = http;
        }

        /// <summary>
        /// ストリームハンドラ
        /// </summary>
        private readonly Dmc::IStreamhandler handler;

        /// <summary>
        /// httpクライアント
        /// </summary>
        private readonly INicoHttp http;

        /// <summary>
        /// ストリーム情報を取得する
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public async Task<Dmc::IStreamInfo> GetStreamInfoAsync(string url)
        {
            var res = await this.http.GetAsync(new Uri(url));
            if (!res.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"プレイリストの取得に失敗しました。(status: {(int)res.StatusCode}, reason_phrase: {res.ReasonPhrase})");
            }
            string content = await res.Content.ReadAsStringAsync();
            string absPath = new Uri(url).AbsoluteUri;
            string baseUrl = absPath.Substring(0, absPath.LastIndexOf('/') + 1);
            if (content.IsNullOrEmpty()) throw new HttpRequestException($"プレイリストファイルの取得に失敗しました。(contentがnullまたは空白です。)");
            return this.handler.GetStreamInfo(content, baseUrl);
        }
    }
}
