using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Extensions.System;
using Niconicome.Extensions.System.List;
using Niconicome.Models.Const;
using Niconicome.Models.Domain.Local.Addons.Core;
using Niconicome.Models.Domain.Local.Addons.Core.Engine;
using Niconicome.Models.Domain.Local.Addons.Core.Installer;
using Niconicome.Models.Domain.Local.Addons.Core.Permisson;
using Niconicome.Models.Domain.Local.IO;
using Niconicome.Models.Domain.Utils;
using Niconicome.Models.Helper.Result;
using Niconicome.Models.Helper.Result.Generic;
using Reactive.Bindings;
using Reactive.Bindings.ObjectExtensions;

namespace Niconicome.Models.Local.Addon
{
    public interface IAddonInstallManager
    {

        /// <summary>
        /// ファイルを選択する
        /// </summary>
        /// <param name="path"></param>
        void Select(string path);

        /// <summary>
        /// アップデートモードにする
        /// </summary>
        void MarkAsUpdate(AddonInfomation infomation);

        /// <summary>
        /// アドオンを読み込む
        /// </summary>
        /// <returns></returns>
        IAttemptResult LoadAddon();

        /// <summary>
        /// アドオンをインストールする
        /// </summary>
        /// <returns></returns>
        IAttemptResult InstallAddon();

        /// <summary>
        /// アドオンを削除する
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        IAttemptResult UnistallAddon(int id);

        /// <summary>
        /// アドオンの説明文字列を取得
        /// </summary>
        /// <returns></returns>
        string GetAddonInfomationString();

        /// <summary>
        /// インストールをキャンセルする
        /// </summary>
        void Cancel();

        /// <summary>
        /// インストールフラグ
        /// </summary>
        ReactiveProperty<bool> IsInstalling { get; }

        /// <summary>
        /// 解凍済みフラグ
        /// </summary>
        ReactiveProperty<bool> IsLoaded { get; }

        /// <summary>
        /// 選択済みフラグ
        /// </summary>
        ReactiveProperty<bool> IsSelected { get; }

        /// <summary>
        /// 読み込んだアドオン情報
        /// </summary>
        ReactiveProperty<AddonInfomation> Infomation { get; }
    }

    public class AddonInstallManager : IAddonInstallManager
    {
        public AddonInstallManager(IAddonInstaller installer, IPermissionsHandler permissionsHandler, IAddonUninstaller uninstaller, IAddonHandler handler)
        {
            this.installer = installer;
            this.permissionsHandler = permissionsHandler;
            this.uninstaller = uninstaller;
            this.handler = handler;
        }

        #region field

        private readonly IAddonInstaller installer;

        private readonly IPermissionsHandler permissionsHandler;

        private readonly IAddonUninstaller uninstaller;

        private readonly IAddonHandler handler;

        private string? tempPath;

        private string? addonPath;

        private AddonInfomation? updateInfo;

        #endregion

        #region Methods

        public void Select(string path)
        {
            this.IsInstalling.Value = true;
            this.addonPath = path;
            this.IsSelected.Value = true;
        }

        public void MarkAsUpdate(AddonInfomation infomation)
        {
            this.updateInfo = infomation;
        }

        public IAttemptResult LoadAddon()
        {

            //解凍
            IAttemptResult<string> extractResult = this.installer.Extract(this.addonPath!);
            if (!extractResult.IsSucceeded || extractResult.Data is null)
            {
                this.Cancel();
                return new AttemptResult() { Message = extractResult.Message, Exception = extractResult.Exception };
            }

            this.tempPath = extractResult.Data;

            IAttemptResult<AddonInfomation> mResult = this.installer.LoadManifest(extractResult.Data);
            if (!mResult.IsSucceeded || mResult.Data is null)
            {
                this.Cancel();
                return new AttemptResult() { Message = mResult.Message, Exception = mResult.Exception };
            }

            this.IsLoaded.Value = true;
            this.Infomation.Value = mResult.Data;
            return new AttemptResult() { IsSucceeded = true };

        }

        public IAttemptResult InstallAddon()
        {
            if (!this.IsLoaded.Value)
            {
                return new AttemptResult() { Message = "アドオンの読み込みが完了していません。" };
            }

            IAttemptResult<AddonInfomation> result = this.installer.Install(this.tempPath!, this.updateInfo);
            if (!result.IsSucceeded)
            {
                return new AttemptResult() { Message = result.Message, Exception = result.Exception };
            }

            this.IsInstalling.Value = false;
            this.updateInfo = null;
            return new AttemptResult() { IsSucceeded = true };

        }

        public IAttemptResult UnistallAddon(int id)
        {
            this.handler.Addons.RemoveAll(addon => addon.ID.Value == id);
            return this.uninstaller.Uninstall(id);
        }

        public string GetAddonInfomationString()
        {
            if (this.Infomation.Value is null) return string.Empty;

            AddonInfomation info = this.Infomation.Value;
            var sb = new StringBuilder();
            sb.AppendLine($"名前：{info.Name.Value}");
            sb.AppendLine($"作者：{info.Author.Value}");
            sb.AppendLine($"説明：{info.Description.Value}");
            sb.AppendLine($"バージョン：{info.Version.Value}");
            sb.AppendLine($"識別子：{info.Identifier.Value}");

            if (info.Permissions.Count > 0)
            {
                sb.AppendLine("権限一覧 " + "-".Repeat(60));

                foreach (var permission in info.Permissions)
                {
                    Permission? data = this.permissionsHandler.GetPermission(permission);

                    if (data is null) continue;

                    sb.AppendLine($"権限名：{data.Name}");
                    sb.AppendLine($"説明：{data.Description}");
                }
            }

            if (info.HostPermissions.Count > 0)
            {
                sb.AppendLine("ホスト権限一覧（アドオンが自由にデータを送受信できるURL）" + "-".Repeat(60));
                sb.AppendLine("！注意！：悪意のあるアドオンは、これらのURLにあなたの情報を送信することができます。適用範囲が広すぎるもの（たとえば、「http://*/」など）はできるだけ許可しないでください。アプリケーションはこのような攻撃に対する防御機構を持ちません。");

                foreach (var host in info.HostPermissions)
                {
                    sb.AppendLine(host);
                }
            }

            return sb.ToString();
        }

        public void Cancel()
        {
            this.IsInstalling.Value = false;
            this.updateInfo = null;
            this.IsLoaded.Value = false;
            this.IsSelected.Value = false;
            this.tempPath = null;
        }


        #endregion

        #region Props

        public ReactiveProperty<bool> IsInstalling { get; init; } = new();

        public ReactiveProperty<bool> IsLoaded { get; init; } = new();

        public ReactiveProperty<bool> IsSelected { get; init; } = new();

        public ReactiveProperty<AddonInfomation> Infomation { get; init; } = new();


        #endregion

        #region private

        #endregion
    }
}
