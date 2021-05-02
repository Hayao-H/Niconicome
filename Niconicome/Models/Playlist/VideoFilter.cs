using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Niconicome.Extensions.System.List;

namespace Niconicome.Models.Playlist
{

    /// <summary>
    /// 動画検索
    /// </summary>
    public interface IVideoFilter
    {
        IEnumerable<IListVideoInfo> FilterVideos(string word, IEnumerable<IListVideoInfo> source, FilterringOptions option = FilterringOptions.None);
    }

    public class VideoFilter : IVideoFilter
    {
        /// <summary>
        /// 動画を検索する
        /// </summary>
        /// <param name="word"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        public IEnumerable<IListVideoInfo> FilterVideos(string word, IEnumerable<IListVideoInfo> source, FilterringOptions option = FilterringOptions.None)
        {
            var result = new List<IListVideoInfo>();
            string[] keywords = Regex.Replace(word, @"[\s]", ",").Split(",");

            if (option != FilterringOptions.OnlyByTag)
            {
                result.AddRange(this.FilterVideoByTitle(keywords, source));
            }
            result.AddRange(this.FilterByTag(keywords, source));

            result = result.Distinct(v => v.Id).ToList();

            return result;
        }

        /// <summary>
        /// タイトルで検索
        /// </summary>
        /// <param name="keywords"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        private IEnumerable<IListVideoInfo> FilterVideoByTitle(string[] keywords, IEnumerable<IListVideoInfo> source)
        {
            var result = new List<IListVideoInfo>();
            foreach (var word in keywords.Select(k=>k.ToLower()))
            {
                result.AddRange(source.Where(v => v.Title.Value.ToLower().Contains(word)));
            }
            result = result.Distinct(v => v.Id).ToList();
            return result;
        }

        /// <summary>
        /// タグで検索
        /// </summary>
        /// <param name="tags"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        private IEnumerable<IListVideoInfo> FilterByTag(string[] tags, IEnumerable<IListVideoInfo> source)
        {
            var result = new List<IListVideoInfo>();
            foreach (var tag in tags)
            {
                result.AddRange(source.Where(v => v.Tags.Contains(tag)));
            }
            result = result.Distinct(v => v.Id).ToList();
            return result;
        }
    }

    public enum FilterringOptions
    {
        None,
        OnlyByTag
    }
}
