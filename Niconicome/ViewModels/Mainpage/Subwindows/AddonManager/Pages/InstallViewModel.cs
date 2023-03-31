using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using Niconicome.Extensions.System;
using Niconicome.Models.Domain.Local.Addons.Core.V2.Engine.Infomation;
using Niconicome.Models.Helper.Result;
using WS = Niconicome.Workspaces.AddonPage;

namespace Niconicome.ViewModels.Mainpage.Subwindows.AddonManager.Pages
{
    public class InstallViewModel
    {
        #region field

        private bool _isInstall;

        private string _id = string.Empty;

        #endregion

        #region Method

        /// <summary>
        /// 初期化
        /// </summary>
        /// <param name="ID"></param>
        public void Initialize(string ID)
        {
            this._isInstall = ID == "0";
            this._id = ID;
            this.InstallOrUpdate = this._isInstall ? "インストール" : "アップデート";
            this.SelectViewDisplay = this._displayBlock;
            this.InfomationViewDisplay = this._displayNone;
            this.InstallViewDisplay = this._displayNone;
            this.IsInstallDisabled = true;
        }

        /// <summary>
        /// ファイルが決定されたとき
        /// </summary>
        public void OnFileSelected()
        {
            this.IsInstallDisabled = true;

            if (string.IsNullOrEmpty(this.SelectedFilePath)) return;

            this.SelectViewDisplay = _displayNone;
            this.InfomationViewDisplay = _displayBlock;

            this.AddonInfomationText = "読み込み中...";
            this.AddonInfomationText = this.GetInfomation();
        }

        /// <summary>
        /// インストール
        /// </summary>
        public void Install()
        {
            if (string.IsNullOrEmpty(this.SelectedFilePath)) return;

            this.InfomationViewDisplay = this._displayNone;
            this.InstallViewDisplay = this._displayBlock;
            this.ModifyPath();

            this.InstallInfomationText = $"{this.InstallOrUpdate}中...";

            IAttemptResult result = this._isInstall? WS.AddonInstallManager.InstallAndLoad(this.SelectedFilePath):WS.AddonInstallManager.UpdateAndLoad(this._id,this.SelectedFilePath);

            var builder = new StringBuilder();
            if (result.IsSucceeded)
            {
                builder.AppendLine($"{this.InstallOrUpdate}に成功しました！");
            }
            else
            {
                builder.AppendLine($"{this.InstallOrUpdate}に失敗しました。");
                builder.AppendLine($"詳細：{result.Message ?? "不明"}");
            }

            this.InstallInfomationText = WS.BlazorHelper.ConvertNewLine(builder.ToString());
            this.IsCompleteDisabled = string.Empty;
        }

        #endregion

        #region Props

        /// <summary>
        /// インストールボタンの有効・無効
        /// </summary>
        public bool IsInstallDisabled { get; private set; }

        /// <summary>
        /// 完了ボタンの有効・無効
        /// </summary>
        public string IsCompleteDisabled { get; private set; } = "disabled";

        /// <summary>
        /// インストールorアップデート
        /// </summary>
        public string InstallOrUpdate { get; private set; } = string.Empty;

        /// <summary>
        /// インストーラーのパス
        /// </summary>
        public string? SelectedFilePath { get; set; }

        /// <summary>
        /// 読み込まれた情報
        /// </summary>
        public string AddonInfomationText { get; private set; } = "Error:未読込";

        /// <summary>
        /// インストール情報
        /// </summary>
        public string InstallInfomationText { get; private set; } = "Error:未読込";

        /// <summary>
        /// 選択画面の表示/非表示
        /// </summary>
        public string SelectViewDisplay { get; private set; } = string.Empty;

        /// <summary>
        /// 情報表示画面の表示/非表示
        /// </summary>
        public string InfomationViewDisplay { get; private set; } = string.Empty;

        /// <summary>
        /// インストール画面の表示/非表示
        /// </summary>
        public string InstallViewDisplay { get; private set; } = string.Empty;

        /// <summary>
        /// display:block
        /// </summary>
        protected readonly string _displayBlock = "d-block";

        /// <summary>
        /// display:none
        /// </summary>
        protected readonly string _displayNone = "d-none";


        #endregion

        #region private

        /// <summary>
        /// アドオンの情報を読み込む
        /// </summary>
        /// <returns></returns>
        private string GetInfomation()
        {
            this.ModifyPath();

            IAttemptResult<IAddonInfomation> result = WS.AddonInstallManager.LoadInfomation(this.SelectedFilePath!);
            if (!result.IsSucceeded || result.Data is null)
            {
                return result.Message ?? "読み込みに失敗しました。";
            }


            IAddonInfomation info = result.Data;

            if (this._isInstall && WS.AddonInstallManager.IsInstalled(info.Identifier))
            {
                IAddonInfomation? dupe = WS.AddonStatusContainer.LoadedAddons.FirstOrDefault(i => i.Identifier == info.Identifier);

                var builder1 = new StringBuilder();
                builder1.AppendLine("すでにインストールされているアドオンであるか、識別子が重複しているためインストールできません。");
                builder1.AppendLine($"識別子：{info.Identifier}");
                builder1.AppendLine($"重複しているアドオン：{dupe?.Name ?? "不明"}");

                return WS.BlazorHelper.ConvertNewLine(builder1.ToString());
            }

            var builder2 = new StringBuilder();
            builder2.AppendLine($"名前：{info.Name}");
            builder2.AppendLine($"作者：{info.Author}");
            builder2.AppendLine($"説明：{info.Description}");
            builder2.AppendLine($"バージョン：{info.Version}");
            builder2.AppendLine($"識別子：{info.Identifier}");

            if (info.Permissions.Count > 0)
            {
                builder2.AppendLine("権限一覧 " + "-".Repeat(60));

                foreach (var permission in info.Permissions)
                {
                    builder2.AppendLine($"権限名：{permission.Name}");
                    builder2.AppendLine($"説明：{permission.Description}");
                }
            }

            if (info.HostPermissions.Count > 0)
            {
                builder2.AppendLine("ホスト権限一覧（アドオンが自由にデータを送受信できるURL）" + "-".Repeat(60));
                builder2.AppendLine("！注意！：悪意のあるアドオンは、これらのURLにあなたの情報を送信することができます。適用範囲が広すぎるもの（たとえば、「http://*/」など）はできるだけ許可しないでください。アプリケーションはこのような攻撃に対する防御機構を持ちません。");

                foreach (var host in info.HostPermissions)
                {
                    builder2.AppendLine(host);
                    builder2.AppendLine("");
                }
            }

                this.IsInstallDisabled = false;
            return WS.BlazorHelper.ConvertNewLine(builder2.ToString());
        }

        /// <summary>
        /// インストールファイルのパスを修正
        /// </summary>
        private void ModifyPath()
        {
            if (string.IsNullOrEmpty(this.SelectedFilePath)) return;

            //「"」を除去
            if (this.SelectedFilePath.StartsWith('"'))
            {
                this.SelectedFilePath = this.SelectedFilePath[1..];
            }
            if (this.SelectedFilePath.EndsWith('"'))
            {
                this.SelectedFilePath = this.SelectedFilePath[..^1];
            }
        }

        #endregion
    }
}
