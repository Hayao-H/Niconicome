using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niconicome.Models.Domain.Niconico.Remote.V2.Search
{

    public enum SearchType
    {
        Tag,
        Keyword
    }

    public enum Genre
    {
        [Query("all")]
        All,
        [Query("hot-topic")]
        HotTopic,
        [Query("entertainment")]
        Entertainment,
        [Query("radio")]
        Radio,
        [Query("music_sound")]
        MusicSound,
        [Query("dance")]
        Dance,
        [Query("animal")]
        Animal,
        [Query("nature")]
        Nature,
        [Query("cooking")]
        Cooking,
        [Query("traveling_outdoor")]
        TravelingOutdoor,
        [Query("vehicle")]
        Vehicle,
        [Query("sports")]
        Sports,
        [Query("society_politics_news")]
        SocietyPoliticsNews,
        [Query("technology_craft")]
        TechnologyCraft,
        [Query("commentary_lecture")]
        CommentaryLecture,
        [Query("anime")]
        Anime,
        [Query("game")]
        Game,
        [Query("other")]
        Other,
        [Query("r18")]
        R18,

    }

    public enum Sort
    {
        [Query("viewCount")]
        ViewCount,
        [Query("mylistCount")]
        MylistCount,
        [Query("commentCount")]
        CommentCount,
        [Query("duration")]
        Length,
        [Query("registeredAt")]
        UploadedTime,
        [Query("lastCommentTime")]
        LastCommentTime,
    }

    public record SortOption(Sort Sort, bool IsAscending);

    public record SearchQuery(SearchType SearchType, Genre Genre, SortOption SortOption, string Query, DateTimeOffset? UploadedDateTimeStart = null, DateTimeOffset? UploadedDateTimeEnd = null, int Page = 1);

    public class QueryAttribute : Attribute
    {
        public QueryAttribute(string query)
        {
            this.Query = query;
        }

        public string Query { get; init; }
    }

    public static class EnumQueryExtension
    {
        public static string GetQuery<E>(this E enumValue) where E : Enum
        {
            var member = enumValue.GetType().GetMember(enumValue.ToString()).First();
            var attr = member.GetCustomAttributes(typeof(QueryAttribute), false).First();
            return ((QueryAttribute)attr).Query;

        }
    }
}
