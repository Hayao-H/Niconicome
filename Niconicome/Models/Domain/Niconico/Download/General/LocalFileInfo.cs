using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niconicome.Models.Domain.Niconico.Download.General
{
    public interface ILocalFileInfo
    {
        /// <summary>
        /// コメント存在フラグ
        /// </summary>
        bool CommentExists { get; }

        /// <summary>
        /// サムネ存在フラグ
        /// </summary>
        bool ThumbExists { get; }

        /// <summary>
        /// 動画存在フラグ
        /// </summary>
        bool VideoExists { get; }

        /// <summary>
        /// 動画情報存在フラグ
        /// </summary>
        bool VideoInfoExist { get; }

        /// <summary>
        /// 市場情報存在フラグ
        /// </summary>
        bool IchibaInfoExist { get; }

        /// <summary>
        /// 動画が別フォルダーに存在する
        /// </summary>
        bool VideoExistInOnotherFolder { get; }

        /// <summary>
        /// 別フォルダーに存在する場合の動画ファイルパス
        /// </summary>
        string VideoFilePath { get; }
    }

    public class LocalFileInfo : ILocalFileInfo
    {
        public bool CommentExists { get; init; }

        public bool ThumbExists { get; init; }

        public bool VideoExists { get; init; }

        public bool VideoInfoExist { get; init; }

        public bool IchibaInfoExist { get; init; }

        public bool VideoExistInOnotherFolder { get; init; }

        public string VideoFilePath { get; init; } = string.Empty;
    }
}
