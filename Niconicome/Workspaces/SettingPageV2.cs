using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Utils;
using Niconicome.Models.Domain.Utils.StringHandler;
using Niconicome.Models.Local.Restore.Import.Niconicome;
using Niconicome.Models.Local.Restore.Import.Xeno;
using Niconicome.Models.Local.State.MessageV2;

namespace Niconicome.Workspaces
{
    public static class SettingPageV2
    {
        /// <summary>
        /// メッセージハンドラ
        /// </summary>
        public static IMessageHandler MessageHandler { get; private set; } = DIFactory.Resolve<IMessageHandler>();

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
    }
}
