using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niconicome.Models.Domain.Niconico.Search
{
    public interface ISearchQuery
    {
        SearchType SearchType { get; }
        Genre Genre { get; }
        Sort Sort { get; }
        string Query { get; }
        bool IsAscending { get; }
        DateTimeOffset? UploadedDateTimeStart { get; }
        DateTimeOffset? UploadedDateTimeEnd { get; }
        int Page { get; }
    }

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

    public class SearchQuery : ISearchQuery
    {
        public SearchType SearchType { get; set; }

        public Genre Genre { get; set; }

        public Sort Sort { get; set; }

        public string Query { get; set; } = string.Empty;

        public bool IsAscending { get; set; }

        public DateTimeOffset? UploadedDateTimeStart { get; set; }

        public DateTimeOffset? UploadedDateTimeEnd { get; set; }

        public int Page { get; set; }
    }
}
