using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niconicome.Models.Domain.Niconico.Remote.Channel
{
    public interface IChannelPageInfo
    {
        bool HasNext { get; }
        string? NextPageQuery { get; }
        string? ChannelName { get; }
        IEnumerable<string> IDs { get; }
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
        public IEnumerable<string> IDs { get; set; } = new List<string>();
    }
}
