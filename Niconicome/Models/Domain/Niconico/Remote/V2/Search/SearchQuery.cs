using System;
using System.Collections.Generic;
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
        All,
        Game,
        MusicSound,
        Anime,
        Entertainment,
        Dance,
        Other,
    }

    public enum Sort
    {
        ViewCount,
        MylistCount,
        CommentCount,
        Length,
        UploadedTime,
        LastCommentTime,
    }

    public record SortOption(Sort Sort, bool IsAscending);

    public record SearchQuery(SearchType SearchType, Genre Genre, SortOption SortOption, string Query, DateTimeOffset? UploadedDateTimeStart = null, DateTimeOffset? UploadedDateTimeEnd = null, int Page = 1);
}
