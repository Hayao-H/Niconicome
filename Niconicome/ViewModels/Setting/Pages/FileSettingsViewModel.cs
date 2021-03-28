using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Local;
using WS = Niconicome.Workspaces;

namespace Niconicome.ViewModels.Setting.Pages
{
    class FileSettingsViewModel : SettingaBase
    {

        public FileSettingsViewModel()
        {
            this.fileFormatField = WS::SettingPage.SettingHandler.GetStringSetting(Settings.FileNameFormat) ?? string.Empty;
            this.defaultFolderField = WS::SettingPage.SettingHandler.GetStringSetting(Settings.DefaultFolder) ?? "downloaded";
            this.isReplaceSBToSBEnableField = WS::SettingPage.SettingHandler.GetBoolSetting(Settings.ReplaceSBToMB);
        }

        private string fileFormatField;

        private string defaultFolderField;

        private bool isReplaceSBToSBEnableField;

        /// <summary>
        /// デフォルトのDLパス
        /// </summary>
        public string DefaultFolder { get => this.defaultFolderField; set => this.Savesetting(ref this.defaultFolderField, value, Settings.DefaultFolder); }

        /// <summary>
        /// ファイル名のフォーマット
        /// </summary>
        public string FileFormat { get => this.fileFormatField; set => this.Savesetting(ref this.fileFormatField, value, Settings.FileNameFormat); }

        /// <summary>
        /// 禁則文字を2バイト文字に置き換える
        /// </summary>
        public bool IsReplaceSBToSBEnable { get => this.isReplaceSBToSBEnableField; set => this.Savesetting(ref this.isReplaceSBToSBEnableField, value, Settings.ReplaceSBToMB); }



    }

    class FileSettingsViewModelD : SettingaBase
    {


        public string DefaultFolder { get; set; } = "download";


        public string FileFormat { get; set; } = "[<id>]<title>";

        public bool IsReplaceSBToSBEnable { get; set; } = true;


    }
}
