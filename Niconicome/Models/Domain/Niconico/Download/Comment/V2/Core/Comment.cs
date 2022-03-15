using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niconicome.Models.Domain.Niconico.Download.Comment.V2.Core
{
    public interface IComment
    {
        /// <summary>
        /// コメント本体
        /// </summary>
        string? Content { get; }

        /// <summary>
        /// コマンド
        /// </summary>
        string? Mail { get; }

        /// <summary>
        /// ユーザーID
        /// </summary>
        string? UserID { get; }

        /// <summary>
        /// Thread
        /// </summary>
        string Thread { get; }

        /// <summary>
        /// コメ番
        /// </summary>
        int No { get; }

        /// <summary>
        /// Fork
        /// </summary>
        int Fork { get; }

        /// <summary>
        /// VPOS
        /// </summary>
        int Vpos { get; }

        /// <summary>
        /// プレ垢フラグ
        /// </summary>
        int Premium { get; }

        /// <summary>
        /// ？
        /// </summary>
        int Anonimity { get; }

        /// <summary>
        /// 削除フラグ？
        /// </summary>
        long Deleted { get; }

        /// <summary>
        /// コメントの種別
        /// </summary>
        CommentType CommentType { get; }
    }

    public class Comment : IComment
    {
        public string? Content { get; init; }

        public string? Mail { get; init; }

        public string? UserID { get; init; }

        public int No { get; init; }

        public string Thread { get; init; } = string.Empty;

        public int Fork { get; init; }

        public int Vpos { get; init; }

        public int Premium { get; init; }

        public int Anonimity { get; init; }

        public long Deleted { get; init; }

        public CommentType CommentType { get; init; }
    }

    public enum CommentType
    {
        Normal,
        Owner,
        Easy,
        Channel
    }
}
