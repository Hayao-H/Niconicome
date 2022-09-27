using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Local.Addons.Core.V2.Engine;
using Niconicome.Models.Domain.Local.Addons.Core.V2.Engine.Infomation;
using Niconicome.Models.Domain.Local.Addons.Core.V2.Engine.JavaScript;
using Niconicome.Models.Domain.Local.IO;
using Niconicome.Models.Domain.Utils;
using Niconicome.Models.Helper.Result;
using Niconicome.Models.Local.Addon.API.Local.Tab;
using TabAPI = Niconicome.Models.Local.Addon.API.Local.Tab;

namespace Niconicome.Models.Domain.Local.Addons.Core.V2.Engine.Context
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

        private bool _isShutDown;

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

        public IAttemptResult ExecuteAddon(IAddonInfomation infomation, APIObjectConatainer container)
        {
            if (this._isExecuted)
            {
                return AttemptResult.Fail("すでに実行されています。");
            }
            else if (this._isShutDown)
            {
                return AttemptResult.Fail("すでにシャットダウンされています。");
            }

            this.AddonInfomation = infomation;
            this._apiCOntainer = container;

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

            //Tab APIにハンドラを設定
            if (container.APIEntryPoint.tab is not null and ITabsManager tab)
            {
                tab.RegisterHandler(item => this.HandlingTabs.Add(item), item => this.HandlingTabs.RemoveAll(i => i == item));
            }

            //APIを追加
            this._engine.AddHostObject("application", container.APIEntryPoint);
            this._engine.AddHostObject("fetch", container.FetchFunc);

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
            if (!this._isExecuted || this._isShutDown)
            {
                return;
            }

            foreach (var tab in this.HandlingTabs)
            {
                if (tab.IsClosed) continue;
                tab.Close();
            }

            this._apiCOntainer?.APIEntryPoint.Dispose();
            this._engine.Dispose();

            this._isExecuted = false;
            this._isShutDown = true;
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
