using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using AngleSharp.Dom;
using Microsoft.ClearScript;
using Microsoft.ClearScript.V8;
using Niconicome.Models.Domain.Niconico.Net.Html;
using Niconicome.Models.Domain.Utils;
using Niconicome.Models.Helper.Result;

namespace Niconicome.Models.Domain.Local.Addons.Core.V2.Engine.JavaScript
{
    public interface IJavaScriptEngine : IDisposable
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

        /// <summary>
        /// スクリプトオブジェクト
        /// </summary>
        dynamic Script { get; }
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

        private readonly List<Timer> _timers = new();

        #endregion

        #region Props

        public dynamic Script => this._engine.Script;

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
            this.InitializeInternal();
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

        public void Dispose()
        {
            this._engine.Dispose();
        }

        #endregion

        #region private



        /// <summary>
        /// 未実装の関数を実装
        /// </summary>
        private void InitializeInternal()
        {
            Action<ScriptObject, int> setTimeout = (function, delay) =>
            {
                var timer = new Timer(delay);
                timer.Elapsed += (sender, _) =>
                {
                    function.Invoke(false);
                    if (sender is not null and Timer tm)
                    {
                        this._timers.Remove(tm);
                    }
                };
                this._timers.Add(timer);
                timer.AutoReset = false;
                timer.Enabled = true;
            };
            this._engine.AddHostObject("setTimeout", setTimeout);

            Func<string, IDocument?> parseHtml = source => HtmlParser.ParseDocument(source);
            this._engine.AddHostObject("parseHtml", parseHtml);
        }

        #endregion
    }
}
