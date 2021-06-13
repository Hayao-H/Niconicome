using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Encodings.Web;
using System.Text.Unicode;

namespace Niconicome.Models.Domain.Niconico.Net.Json
{
    /// <summary>
    /// Json関係の機能を提供するクラス
    /// </summary>
    public static class JsonParser
    {
        /// <summary>
        /// json文字列をデシリアライズする
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static T DeSerialize<T>(string source)
        {
            var options = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = false,
                IgnoreNullValues = true,
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                //DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
            };

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
                options = new JsonSerializerOptions()
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    PropertyNameCaseInsensitive = false,
                    IgnoreNullValues = true,
                    Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                    //DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
                };
            }

            string json = JsonSerializer.Serialize<T>(source, options);

            return json;
        }
    }
}
