using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Playlist;

namespace Niconicome.Models.Domain.Niconico.Remote.Channel
{
    public interface IChannelPageInfo
    {
        /// <summary>
        /// 次のページが存在するかどうか
        /// </summary>
        bool HasNext { get; }

        /// <summary>
        /// 次のページのクエリ
        /// </summary>
        string? NextPageQuery { get; }

        /// <summary>
        /// チャンネル名
        /// </summary>
        string? ChannelName { get; }

        /// <summary>
        /// チャンネル動画一覧
        /// </summary>
        IEnumerable<IListVideoInfo> Videos { get; }
    }

    class ChannelPageInfo : IChannelPageInfo
    {
        /// <summary>
        /// 次ページ存在フラグ//
        /// </summary>
        public bool HasNext { get; set; }

        /// <summary>
        /// 次ページURL
        /// </summary>
        public string? NextPageQuery { get; set; }

        /// <summary>
        /// チャンネル名
        /// </summary>
        public string? ChannelName { get; set; }

        /// <summary>
        /// ID
        /// </summary>
        public IEnumerable<IListVideoInfo> Videos { get; set; } = new List<IListVideoInfo>();
    }
}
