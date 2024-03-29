﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Auth;
using Niconicome.Models.Domain.Local.Settings;
using Niconicome.Models.Domain.Niconico.Download.General;
using Niconicome.Models.Domain.Utils;
using Niconicome.Models.Domain.Utils.StringHandler;
using Niconicome.Models.Local.Application;
using Niconicome.Models.Local.OS;
using Niconicome.Models.Local.Restore;
using Niconicome.Models.Local.Restore.Import.Niconicome;
using Niconicome.Models.Local.Restore.Import.Xeno;
using Niconicome.Models.Local.State;
using Niconicome.Models.Local.State.Style;
using MessageV2 = Niconicome.Models.Local.State.MessageV2;

namespace Niconicome.Workspaces
{
    public static class SettingPageV2
    {
        /// <summary>
        /// メッセージハンドラ
        /// </summary>
        public static MessageV2::IMessageHandler MessageHandler { get; private set; } = DIFactory.Resolve<MessageV2::IMessageHandler>();

        /// <summary>
        /// 文字列ハンドラ
        /// </summary>
        public static IStringHandler StringHandler { get; private set; } = DIFactory.Resolve<IStringHandler>();

        /// <summary>
        /// インポート・エクスポート
        /// </summary>
        public static IImportExportManager ImportExportManager { get; private set; } = DIFactory.Resolve<IImportExportManager>();

        /// <summary>
        /// Xenoからのインポート
        /// </summary>
        public static IXenoImportManager XenoImportManager { get; private set; } = DIFactory.Resolve<IXenoImportManager>();

        /// <summary>
        /// 回復
        /// </summary>
        public static IRestoreManager RestoreManager { get; private set; } = DIFactory.Resolve<IRestoreManager>();

        /// <summary>
        /// ページ
        /// </summary>
        public static IBlazorPageManager BlazorPageManager { get; private set; } = DIFactory.Resolve<IBlazorPageManager>();

        /// <summary>
        /// 設定コンテナ
        /// </summary>
        public static ISettingsContainer SettingsContainer { get; private set; } = DIFactory.Resolve<ISettingsContainer>();

        /// <summary>
        /// 自動ログイン
        /// </summary>
        public static IAutoLogin AutoLogin { get; private set; } = DIFactory.Resolve<IAutoLogin>();

        /// <summary>
        /// userChrome.css
        /// </summary>
        public static IUserChromeHandler UserChromeHandler { get; private set; } = DIFactory.Resolve<IUserChromeHandler>();

        /// <summary>
        /// テーマ
        /// </summary>
        public static IThemehandler Themehandler { get; private set; } = DIFactory.Resolve<IThemehandler>();

        /// <summary>
        /// 電源管理
        /// </summary>
        public static IApplicationPowerManager PowerManager { get; private set; } = DIFactory.Resolve<IApplicationPowerManager>();

        /// <summary>
        /// クリップボード
        /// </summary>
        public static IClipbordManager ClipbordManager { get; private set; } = DIFactory.Resolve<IClipbordManager>();

        /// <summary>
        /// ローカル情報
        /// </summary>
        public static ILocalInfo LocalInfo { get; private set; } = DIFactory.Resolve<ILocalInfo>();

        /// <summary>
        /// ローカルステート
        /// </summary>
        public static ILocalState State { get; private set; } = DIFactory.Resolve<ILocalState>();

        /// <summary>
        /// ファイル名の置き換え
        /// </summary>
        public static IReplaceHandler ReplaceHandler { get; private set; } = DIFactory.Resolve<IReplaceHandler>();

    }
}
