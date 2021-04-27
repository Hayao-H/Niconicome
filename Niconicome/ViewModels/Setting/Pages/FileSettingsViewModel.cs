using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Local.Settings;
using WS = Niconicome.Workspaces;

namespace Niconicome.ViewModels.Setting.Pages
{
    class FileSettingsViewModel : SettingaBase
    {

        public FileSettingsViewModel()
        {
            this.fileFormatField = WS::SettingPage.SettingHandler.GetStringSetting(SettingsEnum.FileNameFormat) ?? string.Empty;
            this.defaultFolderField = WS::SettingPage.SettingHandler.GetStringSetting(SettingsEnum.DefaultFolder) ?? "downloaded";
            this.isReplaceSBToSBEnableField = WS::SettingPage.SettingHandler.GetBoolSetting(SettingsEnum.ReplaceSBToMB);
        }

        private string fileFormatField;

        private string defaultFolderField;

        private bool isReplaceSBToSBEnableField;

        /// <summary>
        /// デフォルトのDLパス
        /// </summary>
        public string DefaultFolder { get => this.defaultFolderField; set => this.Savesetting(ref this.defaultFolderField, value, SettingsEnum.DefaultFolder); }

        /// <summary>
        /// ファイル名のフォーマット
        /// </summary>
        public string FileFormat { get => this.fileFormatField; set => this.Savesetting(ref this.fileFormatField, value, SettingsEnum.FileNameFormat); }

        /// <summary>
        /// 禁則文字を2バイト文字に置き換える
        /// </summary>
        public bool IsReplaceSBToSBEnable { get => this.isReplaceSBToSBEnableField; set => this.Savesetting(ref this.isReplaceSBToSBEnableField, value, SettingsEnum.ReplaceSBToMB); }



    }

    class FileSettingsViewModelD : SettingaBase
    {


        public string DefaultFolder { get; set; } = "download";


        public string FileFormat { get; set; } = "[<id>]<title>";

        public bool IsReplaceSBToSBEnable { get; set; } = true;


    }
}
