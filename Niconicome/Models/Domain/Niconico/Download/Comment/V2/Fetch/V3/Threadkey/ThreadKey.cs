using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Utils;
using Niconicome.Models.Helper.Result;

namespace Niconicome.Models.Domain.Niconico.Download.Comment.V2.Fetch.V3.Threadkey
{
    public interface IThreadKeyInfo
    {
        /// <summary>
        /// スレッドキー
        /// </summary>
        string Key { get; }

        /// <summary>
        /// 有効期限フラグ
        /// </summary>
        bool IsExpired { get; }

        /// <summary>
        /// 初期化
        /// </summary>
        /// <returns></returns>
        Task<IAttemptResult> Initiaize(string videoID);

        /// <summary>
        /// キーを再取得
        /// </summary>
        /// <returns></returns>
        Task<IAttemptResult> RefreshKeyAsync();
    }

    public class ThreadKeyInfo : IThreadKeyInfo
    {
        public ThreadKeyInfo(IThreadKeyHandler handler)
        {
            this._handler = handler;
        }

        public static IThreadKeyInfo New()
        {
            return DIFactory.Resolve<IThreadKeyInfo>();
        }

        #region field

        private readonly IThreadKeyHandler _handler;

        private DateTime _expiredAt;

        private string _videoID = string.Empty;

        #endregion

        #region Props

        public string Key { get; private set; } = string.Empty;

        public bool IsExpired => DateTime.Now > this._expiredAt;

        #endregion

        #region Method

        public async Task<IAttemptResult> Initiaize(string videoID)
        {
            this._videoID = videoID;
            return await this.RefreshKeyAsync();
        }

        public async Task<IAttemptResult> RefreshKeyAsync()
        {

            IAttemptResult<string> result = await this._handler.GetThreadKeyAsync(this._videoID);

            if (!result.IsSucceeded || result.Data is null)
            {
                return result;
            }
            else
            {
                this.Key = result.Data;
                this._expiredAt = DateTime.Now + TimeSpan.FromMinutes(5);
                return AttemptResult.Succeeded();
            }
        }



        #endregion
    }
}
