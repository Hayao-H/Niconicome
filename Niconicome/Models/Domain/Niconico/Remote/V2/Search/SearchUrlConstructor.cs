using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Web;
using Niconicome.Models.Const;
using Niconicome.Models.Domain.Niconico.Net.Json;

namespace Niconicome.Models.Domain.Niconico.Remote.V2.Search
{

    public interface ISearchUrlConstructor
    {
        /// <summary>
        /// 検索URLを取得する
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        string GetUrl(SearchQuery query);
    }

    public class SearchUrlConstructor : ISearchUrlConstructor
    {
        public string GetUrl(SearchQuery query)
        {
            var list = new List<string>
            {
                //クエリ―
                $"q={query.Query}"
            };

            //検索タイプ
            var sType = query.SearchType switch
            {
                SearchType.Tag => "tagsExact",
                _ => "title,description,tags"
            };
            list.Add($"targets={sType}");

            //並び替え
            var sort = query.SortOption.Sort switch
            {
                Sort.ViewCount => "viewCounter",
                Sort.MylistCount => "mylistCounter",
                Sort.CommentCount => "commentCounter",
                Sort.UploadedTime => "startTime",
                Sort.Length => "lengthSeconds",
                Sort.LastCommentTime => "lastCommentTime",
                _ => "viewCounter",
            };
            if (!query.SortOption.IsAscending)
            {
                sort = "-" + sort;
            }
            else
            {
                sort = HttpUtility.UrlEncode("+") + sort;
            }
            list.Add($"_sort={sort}");

            //フィールド
            list.Add("fields=contentId,title,viewCounter,mylistCounter,thumbnailUrl,startTime,commentCounter,likeCounter,lengthSeconds");

            //jsonフィルター
            if (query.Genre != Genre.All || query.UploadedDateTimeStart is not null)
            {
                var wrapper = new JsonFilterWrapper() { Type = "and" };

                //ジャンル
                if (query.Genre != Genre.All)
                {
                    var genre = query.Genre switch
                    {
                        Genre.Game => "ゲーム",
                        Genre.MusicSound => "音楽・サウンド",
                        Genre.Anime => "アニメ",
                        Genre.Entertainment => "エンターテイメント",
                        Genre.Dance => "ダンス",
                        Genre.Other => "その他",
                        _ => "other"
                    };
                    var filter = new JsonFilter()
                    {
                        Type = "equal",
                        Field = "genre",
                        Value = genre
                    };
                    wrapper.Filters.Add(filter);
                }

                //投稿日時
                if (query.UploadedDateTimeStart is not null)
                {
                    var filter = new JsonFilter()
                    {
                        Type = "range",
                        Field = "startTime",
                        From = HttpUtility.UrlEncode(query.UploadedDateTimeStart?.ToString(Format.ISO8601)),
                        To = HttpUtility.UrlEncode((query.UploadedDateTimeEnd ?? DateTimeOffset.Now).ToString(Format.ISO8601)),
                        IncludeLower = true,
                        IncludeUpper = true,
                    };
                    wrapper.Filters.Add(filter);
                }

                var serialized = JsonParser.Serialize(wrapper);
                list.Add($"jsonFilter={serialized}");
            }

            list.Add("_limit=50");
            list.Add($"_offset={50 * (query.Page - 1)}");


            return APIConstant.SnapshotAPIV2 + "?" + string.Join("&", list);
        }

    }

    public class JsonFilterWrapper
    {
        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;

        [JsonPropertyName("filters")]
        public List<JsonFilter> Filters { get; set; } = new();
    }

    /// <summary>
    /// JSONフィルタ
    /// </summary>
    public class JsonFilter
    {
        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;

        [JsonPropertyName("field")]
        public string Field { get; set; } = string.Empty;

        [JsonPropertyName("value")]
        public string? Value { get; set; }

        [JsonPropertyName("from")]
        public string? From { get; set; }

        [JsonPropertyName("to")]
        public string? To { get; set; }

        [JsonPropertyName("include_lower")]
        public bool? IncludeLower { get; set; }

        [JsonPropertyName("include_upper")]
        public bool? IncludeUpper { get; set; }
    }
}
