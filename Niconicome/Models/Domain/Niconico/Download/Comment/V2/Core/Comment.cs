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
        List<string> Command { get; }

        /// <summary>
        /// ユーザーID
        /// </summary>
        string? UserID { get; }

        /// <summary>
        /// Thread
        /// </summary>
        string Thread { get; }

        /// <summary>
        /// Fork
        /// </summary>
        string Fork { get; }

        /// <summary>
        /// コメ番
        /// </summary>
        int No { get; }

        /// <summary>
        /// VPOS
        /// </summary>
        int Vpos { get; }

        /// <summary>
        /// プレ垢フラグ
        /// </summary>
        bool IsPremium { get; }

        /// <summary>
        /// 投コメフラグ
        /// </summary>
        bool IsOwnerComment { get; }

        /// <summary>
        /// ニコられ数
        /// </summary>
        int Nicoru { get; }

        /// <summary>
        /// 共有NGスコア
        /// </summary>
        int Score { get; }

        /// <summary>
        /// 投稿日時
        /// </summary>
        DateTime Date { get; }

        /// <summary>
        /// 投稿日時(Unix時間)
        /// </summary>
        long UnixDate { get; }
    }

    public class Comment : IComment
    {
        public string? Content { get; init; }

        public List<string> Command { get; init; } = new();

        public string? UserID { get; init; }

        public int No { get; init; }

        public string Thread { get; init; } = string.Empty;

        public string Fork { get; init; } = string.Empty;

        public int Vpos { get; init; }

        public bool IsPremium { get; init; }

        public bool IsOwnerComment => this.Fork == "owner";

        public int Nicoru { get; init; }

        public int Score { get; init; }

        public DateTime Date { get; set; }

        //public long UnixDate => (new DateTimeOffset(this.Date, TimeSpan.FromHours(9))).ToUnixTimeSeconds();

        public long UnixDate => (new DateTimeOffset(DateTime.SpecifyKind(this.Date, DateTimeKind.Unspecified), TimeSpan.FromHours(9))).ToUnixTimeSeconds();
        

    }
}
