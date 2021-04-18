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
        string Query { get; }
        ISortOption SortOption { get; }
        DateTimeOffset? UploadedDateTimeStart { get; }
        DateTimeOffset? UploadedDateTimeEnd { get; }
        int Page { get; }
    }

    public interface ISortOption
    {
        bool IsAscending { get; set; }
        Sort Sort { get; set; }
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

    public class SortOption : ISortOption
    {
        public Sort Sort { get; set; }

        public bool IsAscending { get; set; }
    }

    public class SearchQuery : ISearchQuery
    {
        public SearchType SearchType { get; set; }

        public Genre Genre { get; set; }

        public ISortOption SortOption { get; set; } = new SortOption();


        public string Query { get; set; } = string.Empty;

        public DateTimeOffset? UploadedDateTimeStart { get; set; }

        public DateTimeOffset? UploadedDateTimeEnd { get; set; }

        public int Page { get; set; }
    }
}
