using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using Niconicome.Models.Domain.Utils;
using Niconicome.Models.Domain.Niconico.Net.Json;
using Api = Niconicome.Models.Domain.Niconico.Net.Json.API.Search;
using System.DirectoryServices;
using Niconicome.Models.Domain.Local.Store.Types;

namespace Niconicome.Models.Domain.Niconico.Search
{

    public interface ISearchVideo
    {
        string Id { get; }
        string Title { get; }
    }

    public interface ISearchResult
    {
        bool IsSucceeded { get; }
        string? Message { get; }
        IEnumerable<ISearchVideo>? Videos { get; }
    }

    public interface ISearch
    {
        Task<ISearchResult> SearchAsync(ISearchQuery query);
    }

    public interface ISearchClient
    {
        Task<Api::Response> GetResponseAsync(string url);
    }

    /// <summary>
    /// 検索API
    /// </summary>
    class Search : ISearch
    {
        public Search(ISearchClient searchClient, ILogger logger,ISearchUrlConstructor urlConstructor)
        {
            this.client = searchClient;
            this.logger = logger;
            this.urlConstructor = urlConstructor;
        }

        #region DIされるクラス

        private readonly ISearchClient client;

        private readonly ILogger logger;

        private readonly ISearchUrlConstructor urlConstructor;
        #endregion

        /// <summary>
        /// 検索する
        /// </summary>
        /// <param name="query"></param>
        /// <param name="searchType"></param>
        /// <returns></returns>
        public async Task<ISearchResult> SearchAsync(ISearchQuery query)
        {

            string url = this.urlConstructor.GetUrl(query);

            Api::Response data;
            try
            {
                data = await this.client.GetResponseAsync(url);
            }
            catch (Exception e)
            {
                this.logger.Error("スナップショット検索APIへのアクセスに失敗しました。", e);
                return new SearchResult() { Message = $"APIへのアクセスに失敗しました。(詳細: {e.Message})" };
            }

            IEnumerable<ISearchVideo> videos = data.Data.Select(v => new SearchVideo() { Id = v.ContentId, Title = v.Title });

            return new SearchResult() { IsSucceeded = true, Videos = videos };
        }

    }

    /// <summary>
    /// 外部へアクセスする
    /// </summary>
    public class SearchClient : ISearchClient
    {
        public SearchClient(INicoHttp http,ILogger logger)
        {
            this.http = http;
            this.logger = logger;
        }

        #region DIされるクラス
        private readonly ILogger logger;

        private readonly INicoHttp http;
        #endregion

        /// <summary>
        /// APIから検索結果を取得する
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public async Task<Api::Response> GetResponseAsync(string url)
        {
            HttpResponseMessage res;
            try
            {
                res = await this.http.GetAsync(new Uri(url));
            }
            catch (Exception e)
            {
                throw new HttpRequestException($"スナップショット検索APIへのアクセスに失敗しました(詳細: {e.Message})");
            }

            if (!res.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"スナップショット検索APIへのアクセスに失敗しました(詳細: status: {(int)res.StatusCode}, reason_phrase: {res.ReasonPhrase}, url: {url})");
            }

            string rawData = await res.Content.ReadAsStringAsync();
            Api::Response data;

            try
            {
                data = JsonParser.DeSerialize<Api::Response>(rawData);
            }
            catch (Exception e)
            {
                throw new JsonException($"検索結果の解析に失敗しました。(詳細: {e.Message})");
            }
            this.logger.Log($"{url}から{data.Data.Count}件の検索結果を取得しました。");

            return data;
        }

    }

    /// <summary>
    /// 検索結果
    /// </summary>
    public class SearchResult : ISearchResult
    {
        public bool IsSucceeded { get; set; }

        public string? Message { get; set; }

        public IEnumerable<ISearchVideo>? Videos { get; set; } = new List<ISearchVideo>();
    }

    /// <summary>
    /// 動画情報
    /// </summary>
    public class SearchVideo : ISearchVideo
    {
        public string Id { get; set; } = string.Empty;

        public string Title { get; set; } = string.Empty;
    }
}
