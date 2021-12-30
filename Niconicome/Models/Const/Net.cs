using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niconicome.Models.Const
{
    class Net
    {
        /// <summary>
        /// ニコニコ動画視聴ページのURL
        /// </summary>
        public const string NiconicoWatchUrl = "https://nicovideo.jp/watch/";

        /// <summary>
        /// ニコニコの短縮リンク
        /// </summary>
        public const string NiconicoShortUrl = "https://nico.ms/";

        /// <summary>
        /// 削除動画のサムネ
        /// </summary>
        public const string NiconicoDeletedVideothumb = @"https://nicovideo.cdn.nimg.jp/web/img/common/video_deleted.jpg";

        /// <summary>
        /// エコノミー判定されない視聴回数
        /// </summary>
        public static int EconomyAvoidableViewCount = 500;

        /// <summary>
        /// 同時取得数の上限（デフォルト）
        /// </summary>
        public static int DefaultMaxParallelFetchCount = 3;

        /// <summary>
        /// 動画取得の待機時間（デフォルト）
        /// </summary>
        public static int DefaultFetchWaitInterval = 5;
    }
}
