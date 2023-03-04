using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Playlist;

namespace Niconicome.Models.Playlist.V2.Utils
{
    public interface IVideoInfoFilterManager
    {
        /// <summary>
        /// タグで絞り込み
        /// </summary>
        /// <param name="source"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        IReadOnlyList<IVideoInfo> FilterByTags(IReadOnlyList<IVideoInfo> source, string query);

        /// <summary>
        /// キーワードで絞り込み
        /// </summary>
        /// <param name="source"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        IReadOnlyList<IVideoInfo> FilterByKeyWord(IReadOnlyList<IVideoInfo> source, string query);
    }

    public class VideoInfoFilterManager : IVideoInfoFilterManager
    {
        #region Method

        public IReadOnlyList<IVideoInfo> FilterByTags(IReadOnlyList<IVideoInfo> source, string query)
        {
            IEnumerable<IVideoInfo> matched = source;
            query = Regex.Replace(query, @"\s", ",");
            string[] queries = query.Split(',');

            foreach (var tag in queries)
            {
                matched = matched.Where(v => v.Tags.Select(t => t.Name).Any(t => t == tag));
            }

            return matched.ToList().AsReadOnly();
        }

        public IReadOnlyList<IVideoInfo> FilterByKeyWord(IReadOnlyList<IVideoInfo> source, string query)
        {
            IEnumerable<IVideoInfo> matched = source;
            query = Regex.Replace(query, @"\s", ",");
            string[] queries = query.Split(',');

            foreach (var keyword in queries)
            {
                matched = matched.Where(v => this.IsMatchWithKeyword(v, keyword));
            }

            return matched.ToList().AsReadOnly();
        }

        #endregion

        #region private

        private bool IsMatchWithKeyword(IVideoInfo video, string keyword)
        {
            if (video.Tags.Select(v => v.Name).Contains(keyword))
            {
                return true;
            }

            if (video.Title.Contains(keyword))
            {
                return true;
            }

            if (video.OwnerName.Contains(keyword))
            {
                return true;
            }

            return false;
        }

        #endregion

    }
}
