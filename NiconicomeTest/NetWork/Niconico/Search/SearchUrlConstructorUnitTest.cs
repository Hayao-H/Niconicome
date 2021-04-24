using System;
using System.Web;
using Niconicome.Models.Const;
using Niconicome.Models.Domain.Niconico.Search;
using NUnit.Framework;

namespace NiconicomeTest.NetWork.Niconico.Search
{
    class SearchUrlConstructorUnitTest
    {
        private ISearchUrlConstructor? searchUrlConstructor;

        [SetUp]
        public void SetUp()
        {
            this.searchUrlConstructor = new SearchUrlConstructor();
        }

        [Test]
        public void タグ検索()
        {
            var query = new SearchQuery() { SearchType = SearchType.Tag, Query = "東方", Page = 1, SortOption = new SortOption() { Sort = Sort.ViewCount } };
            var result = this.searchUrlConstructor!.GetUrl(query);
            var expected = API.SnapshotAPIV2 + "?" + "q=東方&targets=tags&_sort=-viewCounter&fields=contentId,title,viewCounter,mylistCounter,thumbnailUrl,startTime,commentCounter&_limit=50&_offset=0";

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void タグ検索降順()
        {
            var query = new SearchQuery() { SearchType = SearchType.Tag, Query = "東方", Page = 1, SortOption = new SortOption() { Sort = Sort.ViewCount, IsAscending = true } };
            var result = this.searchUrlConstructor!.GetUrl(query);
            var expected = API.SnapshotAPIV2 + "?" + "q=東方&targets=tags&_sort=viewCounter&fields=contentId,title,viewCounter,mylistCounter,thumbnailUrl,startTime,commentCounter&_limit=50&_offset=0";

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void タグ検索2ページ目()
        {
            var query = new SearchQuery() { SearchType = SearchType.Tag, Query = "東方", Page = 2, SortOption = new SortOption() { Sort = Sort.ViewCount } };
            var result = this.searchUrlConstructor!.GetUrl(query);
            var expected = API.SnapshotAPIV2 + "?" + "q=東方&targets=tags&_sort=-viewCounter&fields=contentId,title,viewCounter,mylistCounter,thumbnailUrl,startTime,commentCounter&_limit=50&_offset=50";

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void キーワード検索()
        {
            var query = new SearchQuery() { SearchType = SearchType.Keyword, Query = "東方", Page = 1, SortOption = new SortOption() { Sort = Sort.ViewCount } };
            var result = this.searchUrlConstructor!.GetUrl(query);
            var expected = API.SnapshotAPIV2 + "?" + "q=東方&targets=title,description,tags&_sort=-viewCounter&fields=contentId,title,viewCounter,mylistCounter,thumbnailUrl,startTime,commentCounter&_limit=50&_offset=0";

            Assert.That(result, Is.EqualTo(expected));
        }


        [Test]
        public void Jsonフィルタータグゲームジャンル()
        {
            var query = new SearchQuery() { SearchType = SearchType.Tag, Query = "東方", Page = 1, SortOption = new SortOption() { Sort = Sort.ViewCount }, Genre = Genre.Game };
            var result = this.searchUrlConstructor!.GetUrl(query);
            var expected = API.SnapshotAPIV2 + "?" + "q=東方&targets=tags&_sort=-viewCounter&fields=contentId,title,viewCounter,mylistCounter,thumbnailUrl,startTime,commentCounter&jsonFilter={\"type\":\"and\",\"filters\":[{\"type\":\"equal\",\"field\":\"genre\",\"value\":\"ゲーム\"}]}&_limit=50&_offset=0";

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void Jsonフィルタータグ投稿日時()
        {
            var query = new SearchQuery() { SearchType = SearchType.Tag, Query = "東方", Page = 1, SortOption = new SortOption() { Sort = Sort.ViewCount }, UploadedDateTimeStart = new DateTimeOffset(2021, 4, 18, 0, 0, 0, TimeSpan.FromHours(9)), UploadedDateTimeEnd = new DateTimeOffset(2021, 4, 18, 0, 0, 0, TimeSpan.FromHours(9)) };
            var result = this.searchUrlConstructor!.GetUrl(query);
            var expected = API.SnapshotAPIV2 + "?" + "q=東方&targets=tags&_sort=-viewCounter&fields=contentId,title,viewCounter,mylistCounter,thumbnailUrl,startTime,commentCounter&jsonFilter={\"type\":\"and\",\"filters\":[{\"type\":\"range\",\"field\":\"startTime\",\"from\":\"" + HttpUtility.UrlEncode("2021-04-18T00:00:00+09:00") + "\",\"to\":\"" + HttpUtility.UrlEncode("2021-04-18T00:00:00+09:00") + "\",\"include_lower\":true,\"include_upper\":true}]}&_limit=50&_offset=0";

            Assert.That(result, Is.EqualTo(expected));
        }
    }
}
