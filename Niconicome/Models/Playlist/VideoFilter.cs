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
        IEnumerable<ITreeVideoInfo> FilterVideos(string word, IEnumerable<ITreeVideoInfo> source, FilterringOptions option = FilterringOptions.None);
    }

    public class VideoFilter : IVideoFilter
    {
        /// <summary>
        /// 動画を検索する
        /// </summary>
        /// <param name="word"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        public IEnumerable<ITreeVideoInfo> FilterVideos(string word, IEnumerable<ITreeVideoInfo> source, FilterringOptions option = FilterringOptions.None)
        {
            var result = new List<ITreeVideoInfo>();
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
        private IEnumerable<ITreeVideoInfo> FilterVideoByTitle(string[] keywords, IEnumerable<ITreeVideoInfo> source)
        {
            var result = new List<ITreeVideoInfo>();
            foreach (var word in keywords.Select(k=>k.ToLower()))
            {
                result.AddRange(source.Where(v => v.Title.ToLower().Contains(word)));
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
        private IEnumerable<ITreeVideoInfo> FilterByTag(string[] tags, IEnumerable<ITreeVideoInfo> source)
        {
            var result = new List<ITreeVideoInfo>();
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
