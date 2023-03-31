using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using System.Text.Json.Serialization.Metadata;

namespace Niconicome.Models.Domain.Niconico.Net.Json
{
    /// <summary>
    /// Json関係の機能を提供するクラス
    /// </summary>
    public static class JsonParser
    {
        public static JsonSerializerOptions DefaultOption =>
                new JsonSerializerOptions()
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    AllowTrailingCommas = true,
                    PropertyNameCaseInsensitive = false,
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                    Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
                };

        /// <summary>
        /// json文字列をデシリアライズする
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static T DeSerialize<T>(string source)
        {
            var options = JsonParser.DefaultOption;

            T? instance = JsonSerializer.Deserialize<T>(source, options);
            //nullチェック
            if (instance == null) throw new JsonException("jsonのデシリアライズに失敗しました。");

            return instance;
        }

        /// <summary>
        /// オブジェクトをJsonにシリアライズする
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string Serialize<T>(T source, JsonSerializerOptions? options = null)
        {

            if (options is null)
            {
                options = DefaultOption;
            }

            string json = JsonSerializer.Serialize<T>(source, options);

            return json;
        }

    }
}
