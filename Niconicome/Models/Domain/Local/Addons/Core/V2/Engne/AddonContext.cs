using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Local.IO;
using Niconicome.Models.Domain.Utils;
using Niconicome.Models.Helper.Result;
using TabAPI = Niconicome.Models.Local.Addon.API.Local.Tab;

namespace Niconicome.Models.Domain.Local.Addons.Core.V2.Engne
{
    public interface IAddonContext
    {
        /// <summary>
        /// 開いているタブ
        /// </summary>
        List<TabAPI::ITabItem> HandlingTabs { get; }

        /// <summary>
        /// アドオン情報
        /// </summary>
        IAddonInfomation? AddonInfomation { get; }

        /// <summary>
        /// 初期化してスクリプトを実行
        /// </summary>
        /// <param name="infomation"></param>
        /// <returns></returns>
        IAttemptResult ExecuteAddon(IAddonInfomation infomation, APIObjectConatainer conatainer);

        /// <summary>
        /// アドオンを終了する
        /// </summary>
        void ShutDown();

        /// <summary>
        /// 例外ハンドラ
        /// </summary>
        /// <param name="handler">例外ハンドラ(sender,exception)</param>
        void RegisterExceptionHandler(Action<IAddonContext, Exception> handler);
    }

    public class AddonContext : IAddonContext, IDisposable
    {
        public AddonContext(
            IJavaScriptEngine engine, INicoFileIO fileIO, ILogger logger)
        {
            this._engine = engine;
            this._fileIO = fileIO;
            this._logger = logger;
        }

        #region field

        private readonly IJavaScriptEngine _engine;

        private readonly INicoFileIO _fileIO;

        private readonly ILogger _logger;

        private bool _isExecuted;

        private readonly List<Action<IAddonContext, Exception>> _exceptionHandlers = new();

        private APIObjectConatainer? _apiCOntainer;

        #endregion

        #region Props

        public IAddonInfomation? AddonInfomation { get; private set; }

        public List<TabAPI::ITabItem> HandlingTabs { get; init; } = new();


        #endregion

        #region Methods

        public void RegisterExceptionHandler(Action<IAddonContext, Exception> handler)
        {
            this._exceptionHandlers.Add(handler);
        }

        public IAttemptResult ExecuteAddon(IAddonInfomation infomation, APIObjectConatainer conatainer)
        {
            if (this._isExecuted)
            {
                return AttemptResult.Fail("すでに実行されています。");
            }

            this.AddonInfomation = infomation;
            this._apiCOntainer = conatainer;

            //スクリプト読み込み
            string script;
            try
            {
                script = this._fileIO.OpenRead(this.AddonInfomation.ScriptPath);
            }
            catch (Exception ex)
            {
                this._logger.Error("スクリプトの読み込みに失敗しました。", ex);
                return AttemptResult.Fail($"スクリプトの読み込みに失敗しました。（詳細：{ex.Message}）");
            }

            //APIを追加
            this._engine.AddHostObject("application", conatainer.APIEntryPoint);
            this._engine.AddHostObject("fetch", conatainer.FetchFunc);

            _ = Task.Run(() =>
            {
                try
                {
                    this._engine.Execute(script);
                }
                catch (Exception ex)
                {
                    this._logger.Error("スクリプト実行時にエラーが発生しました。", ex);
                    this.OnError(ex);
                }
            });

            this._isExecuted = true;
            return AttemptResult.Succeeded();


        }

        public void ShutDown()
        {
            if (!this._isExecuted)
            {
                return;
            }

            foreach (var tab in this.HandlingTabs)
            {
                if (tab.IsClosed) continue;
                tab.Close();
            }

            this._apiCOntainer?.APIEntryPoint.Dispose();
            this._isExecuted = false;
        }


        #endregion

        #region private

        private void OnError(Exception ex)
        {
            foreach (var handler in this._exceptionHandlers)
            {
                try
                {
                    handler(this, ex);
                }
                catch { }
            }
        }

        #endregion

        public void Dispose()
        {
            this._exceptionHandlers.Clear();
            this.ShutDown();
        }
    }
}
