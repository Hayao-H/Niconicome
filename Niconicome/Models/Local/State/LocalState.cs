using Niconicome.Models.Domain.Local.Server.Core;
using Niconicome.Models.Domain.Local.Settings;
using Niconicome.Models.Domain.Utils.NicoLogger;
using Niconicome.Models.Helper.Result;
using Niconicome.Models.Local.Settings;

namespace Niconicome.Models.Local.State
{

    public interface ILocalState
    {
        /// <summary>
        /// 設定ウィンドウ
        /// </summary>
        bool IsSettingWindowOpen { get; set; }

        /// <summary>
        /// デバッグフラグ
        /// </summary>
        bool IsDebugMode { get; set; }

        /// <summary>
        /// インポート中
        /// </summary>
        bool IsImportingFromXeno { get; set; }

        /// <summary>
        /// アドオンマネージャー
        /// </summary>
        bool IsAddonManagerOpen { get; set; }

        /// <summary>
        /// ダウンロードタスク一覧
        /// </summary>
        bool IsTaskWindowOpen { get; set; }

        /// <summary>
        /// 設定タブ
        /// </summary>
        bool IsSettingTabOpen { get; set; }

        /// <summary>
        /// ローカルサーバーのポート
        /// </summary>
        int Port { get; }
    }

    public class LocalState : ILocalState
    {
        public LocalState(INiconicomeLogger logger, IServer server)
        {
            this._logger = logger;
            this._server = server;

        }

        #region field

        private readonly INiconicomeLogger _logger;

        private readonly IServer _server;

        private bool _isDebugMode;

        #endregion

        #region Props
        public bool IsSettingWindowOpen { get; set; }

        public bool IsDebugMode
        {
            get => this._isDebugMode;
            set
            {
                this._isDebugMode = value;
                this._logger.IsDebugMode = value;
            }
        }

        public bool IsImportingFromXeno { get; set; }

        public bool IsAddonManagerOpen { get; set; }

        public bool IsTaskWindowOpen { get; set; }

        public bool IsSettingTabOpen { get; set; }

        public int Port => this._server.Port;

        #endregion
    }
}
