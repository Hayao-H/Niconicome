using System;
using System.Net.Http;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.Extensions.DependencyInjection;
using Niconicome.Models.Domain.Niconico;
using System.Threading.Tasks;
using Utils = Niconicome.Models.Domain.Utils;

namespace Niconicome.Models.Domain.Network
{

    public interface ICacheHandler
    {
        string GetDirectoryName(string id,string defaultDirName="0");
        string GetCachePath(string id, CacheType cacheType);
        bool HasCache(string id, CacheType cacheType);
       Task<string> GetOrCreateCacheAsync(string id, CacheType cacheType, string url,bool overwrite = false);
    }

    public interface ICacheStraem
    {
        void WriteCache(byte[] data, string filePath);
        Task GetAndWrite(string url, string filePath);
        Task<bool> TryGetAndWrite(string url, string filePath);
    }

    /// <summary>
    /// キャッシュを司るクラス
    /// 〇配置
    /// 実行ディレクトリ-chache-<各種ファイルタイプ>-<mod計算値>-<ファイル実態>
    /// </summary>
    public class CacheHandler:ICacheHandler
    {

        public CacheHandler(ICacheStraem cacheStraem)
        {
            this.straem = cacheStraem;
        }

        private readonly ICacheStraem straem;

        /// <summary>
        /// ディレクトリー名を導出する(n mod 20で計算)
        /// </summary>
        /// <param name="id"></param>
        /// <param name="defaultDirName"></param>
        /// <returns></returns>
        public string GetDirectoryName(string id,string defaultDirName="0")
        {
            string numberOnly = Regex.Replace(id, @"\D", "");
            int number;
            try
            {
                number = int.Parse(numberOnly);
            }
            catch
            {
                return defaultDirName;
            }

            return (number % 20).ToString();
        }

        /// <summary>
        /// キャッシュのパスを取得する
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cacheType"></param>
        /// <returns></returns>
        public string GetCachePath(string id, CacheType cacheType)
        {
            string dirName = this.GetDirectoryName(id);
            string typeDir = this.GetTypeDirectoryName(cacheType);
            string ext = this.GetFileExtension(cacheType);

            return Path.Combine("cache", typeDir, dirName, id+ext);

        }

        /// <summary>
        /// キャッシュが存在するかどうかを返す
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cacheType"></param>
        /// <returns></returns>
        public bool HasCache(string id, CacheType cacheType)
        {
            string path = this.GetCachePath(id, cacheType);
            return File.Exists(path);
        }

        /// <summary>
        /// キャッシュファイルを作成または取得する
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cacheType"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public async Task<string> GetOrCreateCacheAsync(string id, CacheType cacheType, string url,bool overwrite=false)
        {
            string path = this.GetCachePath(id, cacheType);

            if (!this.HasCache(id, cacheType)||overwrite)
            {
                bool result = await this.straem.TryGetAndWrite(url, path);
                if (!result) return string.Empty;
                return path;
            } else
            {
                return path;
            }
        }


        /// <summary>
        /// キャッシュの種類に応じてディレクトリー名を返す
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private string GetTypeDirectoryName(CacheType type)
        {
            return type switch
            {
                CacheType.Thumbnail => "thumb",
                _ => "nonetype"
            };
        }

        /// <summary>
        /// キャッシュの種類に応じて拡張子コンマ付きでを返す
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private string GetFileExtension(CacheType type)
        {
            return type switch
            {
                CacheType.Thumbnail => ".jpg",
                _ => string.Empty
            };
        }
    }

    public class CacheStream : ICacheStraem
    {

        public CacheStream(INicoHttp http)
        {
            this.http = http;
        }

        /// <summary>
        /// httpクライアント
        /// </summary>
        private readonly INicoHttp http;

        /// <summary>
        /// キャッシュデータを書き込む
        /// </summary>
        /// <param name="data"></param>
        /// <param name="filePath"></param>
        public void WriteCache(byte[] data, string filePath)
        {
            string? dir = Path.GetDirectoryName(filePath);

            if (dir is null) return;

            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            //バッファーサイズ(10MB)
            const int bufferSize = 10 * 1024 * 1024;
            using var fs = File.Create(filePath,bufferSize);
            fs.Write(data);
        }

        /// <summary>
        /// ネットワークからキャッシュを取得して書き込む
        /// </summary>
        /// <param name="url"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public async Task GetAndWrite(string url, string filePath)
        {

            if (url is null) throw new InvalidOperationException($"urlがnullのため、キャッシュリソースを取得できません");
            if (url == string.Empty) throw new InvalidOperationException($"urlが空白のため、キャッシュリソースを取得できません");
            if (filePath is null) throw new InvalidOperationException($"filePathがnullのため、キャッシュリソースを取得できません");
            if (filePath == string.Empty) throw new InvalidOperationException($"filePathが空白のため、キャッシュリソースを取得できません");

            var res = await this.http.GetAsync(new Uri(url));

            if (!res.IsSuccessStatusCode) throw new HttpRequestException($"リソース'{url}'の取得に失敗しました。(詳細: status_code:{(int)res.StatusCode}, reason_phrase:{res.ReasonPhrase} )");

            var data = await res.Content.ReadAsByteArrayAsync();

            this.WriteCache(data, filePath);
        }

        /// <summary>
        /// ネットワークからキャッシュを安全に取得する
        /// </summary>
        /// <param name="url"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public async Task<bool> TryGetAndWrite(string url, string filePath)
        {
            try
            {
               await this.GetAndWrite(url, filePath);
            }
            catch (Exception e)
            {
                var logger = Utils::DIFactory.Provider.GetRequiredService<Utils::ILogger>();
                logger.Error($"キャッシュ用データの取得・書き込みに失敗しました。(url:{url}, path:{filePath})",e);
                return false;
            }

            return true;
        }



    }


    /// <summary>
    /// キャッシュの形式
    /// </summary>
    public enum CacheType
    {
        Thumbnail
    }
}
