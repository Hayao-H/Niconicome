using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ClearScript;
using Niconicome.Extensions;
using Niconicome.Models.Domain.Niconico.Video.Infomations;
using Niconicome.Models.Domain.Utils.Error;
using Niconicome.Models.Helper.Result;
using Error = Niconicome.Models.Domain.Local.Addons.API.Hooks.HooksManagerError;

namespace Niconicome.Models.Domain.Local.Addons.API.Hooks
{
    public interface IHooksManager
    {
        /// <summary>
        /// 視聴ページを解析する
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        IAttemptResult<dynamic> ParseWatchPage(string page);

        /// <summary>
        /// 動画情報を取得する
        /// </summary>
        /// <param name="videoID"></param>
        /// <returns></returns>
        Task<IAttemptResult<dynamic>> GetVideoInfoAsync(string videoID);

        /// <summary>
        /// セッションを確立する
        /// </summary>
        /// <param name="dmcInfo"></param>
        /// <returns></returns>
        Task<IAttemptResult<dynamic>> EnsureSessionAsync(dynamic dmcInfo);

        /// <summary>
        /// 関数を登録する
        /// </summary>
        /// <param name="function"></param>
        /// <param name="type"></param>
        void Register(ScriptObject function, HookType type);

        /// <summary>
        /// 指定したタイプの関数が登録されているかどうかチェックする
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        bool IsRegistered(HookType type);
    }

    public class HooksManager : IHooksManager
    {
        public HooksManager(IErrorHandler errorHandler)
        {
            this._errorHandler = errorHandler;
        }

        #region field


        private readonly IErrorHandler _errorHandler;

        private Dictionary<HookType, ScriptObject> hooks = new();

        #endregion

        #region Method

        public IAttemptResult<dynamic> ParseWatchPage(string page)
        {
            this.hooks.TryGetValue(HookType.WatchPageParser, out ScriptObject? function);

            if (function is null)
            {
                this._errorHandler.HandleError(Error.PageAnalyzeFunctionNotRegistered);
                return AttemptResult<dynamic>.Fail(this._errorHandler.GetMessageForResult(Error.PageAnalyzeFunctionNotRegistered));
            }

            dynamic returnVal;
            try
            {
                returnVal = function.Invoke(false, page);
            }
            catch (Exception ex)
            {
                this._errorHandler.HandleError(Error.FailedToAnalyzeWatchPage, ex);
                return AttemptResult<dynamic>.Fail(this._errorHandler.GetMessageForResult(Error.FailedToAnalyzeWatchPage, ex));
            }

            return AttemptResult<dynamic>.Succeeded(returnVal);

        }

        public async Task<IAttemptResult<dynamic>> GetVideoInfoAsync(string videoID)
        {
            this.hooks.TryGetValue(HookType.VIdeoInfoFetcher, out ScriptObject? function);

            if (function is null)
            {
                this._errorHandler.HandleError(Error.VideoInfoFunctionNotRegistered);
                return AttemptResult<dynamic>.Fail(this._errorHandler.GetMessageForResult(Error.VideoInfoFunctionNotRegistered));
            }

            dynamic returnVal;
            var id = this.GetTrackID();

            try
            {
                returnVal = await function.Invoke(false, videoID, id).As<Task<dynamic>>();
            }
            catch (Exception ex)
            {
                this._errorHandler.HandleError(Error.FailedToAnalyzeWatchPage, ex);
                return AttemptResult<dynamic>.Fail(this._errorHandler.GetMessageForResult(Error.FailedToFetchVideoInfo, ex));
            }

            return AttemptResult<dynamic>.Succeeded(returnVal);
        }


        public async Task<IAttemptResult<dynamic>> EnsureSessionAsync(dynamic dmcInfo)
        {
            this.hooks.TryGetValue(HookType.SessionEnsuring, out ScriptObject? function);

            if (function is null)
            {
                this._errorHandler.HandleError(Error.SessionFunctionNotRegistered);
                return AttemptResult<dynamic>.Fail(this._errorHandler.GetMessageForResult(Error.SessionFunctionNotRegistered));
            }

            dynamic returnVal;

            try
            {
                returnVal = await function.Invoke(false, dmcInfo);
            }
            catch (Exception ex)
            {
                this._errorHandler.HandleError(Error.FailedToEnsureSession, ex);
                return AttemptResult<dynamic>.Fail(this._errorHandler.GetMessageForResult(Error.FailedToEnsureSession, ex));
            }

            return AttemptResult<dynamic>.Succeeded(returnVal);
        }


        public void Register(ScriptObject function, HookType type)
        {
            if (this.hooks.ContainsKey(type))
            {
                this.hooks[type] = function;
            }
            else
            {
                this.hooks.Add(type, function);
            }
        }

        public bool IsRegistered(HookType type)
        {
            return this.hooks.ContainsKey(type) && this.hooks[type] is not null;
        }


        #endregion

        #region private
        private string GetTrackID()
        {
            var source = new List<string>();
            source.AddRange(Enumerable.Range('a', 26).Select(x => ((char)x).ToString()));
            source.AddRange(Enumerable.Range('A', 26).Select(x => ((char)x).ToString()));
            source.AddRange(Enumerable.Range(0, 10).Select(x => x.ToString()));

            var ramd = new Random();
            var id = new StringBuilder();

            foreach (var _ in Enumerable.Range(0, 10))
            {
                id.Append(source[ramd.Next(26 + 26 + 10 - 1)]);
            }

            id.Append("_");

            id.Append(ramd.NextInt64((long)Math.Pow(10, 12), (long)Math.Pow(10, 13)).ToString());

            return id.ToString();
        }

        #endregion
    }

    public enum HookType
    {
        WatchPageParser,
        SessionEnsuring,
        VIdeoInfoFetcher,
    }
}
