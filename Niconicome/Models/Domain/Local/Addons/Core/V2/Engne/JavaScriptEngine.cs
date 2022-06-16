using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ClearScript.V8;
using Niconicome.Models.Domain.Utils;
using Niconicome.Models.Helper.Result;

namespace Niconicome.Models.Domain.Local.Addons.Core.V2.Engne
{
    public interface IJavaScriptEngine
    {
        /// <summary>
        /// エンジンの初期化
        /// </summary>
        /// <param name="isDebuggingEnable">デバッグフラグ</param>
        void Initialize(bool isDebuggingEnable);

        /// <summary>
        /// スクリプトを実行する
        /// </summary>
        /// <param name="script"></param>
        /// <returns></returns>
        IAttemptResult Execute(string script);

        /// <summary>
        /// ホストオブジェクトを追加
        /// </summary>
        /// <param name="name"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        IAttemptResult AddHostObject(string name, object obj);
    }

    public class JavaScriptEngine : IJavaScriptEngine
    {
        public JavaScriptEngine(ILogger logger)
        {
            this._logger = logger;
        }

        #region field

        private V8ScriptEngine _engine = new();

        private readonly ILogger _logger;

        #endregion

        #region Method

        public IAttemptResult Execute(string script)
        {
            try
            {
                this._engine.Execute(script);
            }
            catch (Exception ex)
            {
                this._logger.Error("JavaScript実行中にエラーが発生しました。", ex);
                return AttemptResult.Fail($"JavaScript実行中にエラーが発生しました。（詳細：{ex.Message}）");
            }

            return AttemptResult.Succeeded();
        }

        public void Initialize(bool isDebuggingEnable)
        {
            this._engine = isDebuggingEnable ? new V8ScriptEngine(JavaScriptEngineFlags.DebugMode) : new V8ScriptEngine(JavaScriptEngineFlags.Default);

        }

        public IAttemptResult AddHostObject(string name, object obj)
        {
            try
            {
                this._engine.AddHostObject(name, obj);

            }
            catch (Exception ex)
            {
                this._logger.Error("ホストオブジェクトの追加でエラーが発生しました。", ex);
                return AttemptResult.Fail($"ホストオブジェクトの追加でエラーが発生しました。（詳細：{ex.Message}）");
            }

            return AttemptResult.Succeeded();
        }


        #endregion
    }
}
