using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Extensions.System;
using Niconicome.Models.Const;
using Niconicome.Models.Domain.Local.External;
using Niconicome.Models.Helper.Result;
using Niconicome.Models.Local.Settings;
using Niconicome.Models.Playlist;
using Niconicome.Models.Playlist.VideoList;

namespace Niconicome.Models.Local.External
{
    public interface IExternalAppUtils
    {
        IAttemptResult OpenInPlayerA(IListVideoInfo videoInfo);
        IAttemptResult OpenInPlayerB(IListVideoInfo videoInfo);
        IAttemptResult SendToAppA(IListVideoInfo videoInfo);
        IAttemptResult SendToAppB(IListVideoInfo videoInfo);
    }

    class ExternalAppUtils : IExternalAppUtils
    {
        public ExternalAppUtils(ICurrent current, ICommandExecuter commandExecuter, ILocalSettingHandler localSettingHandler)
        {
            this.current = current;
            this.commandExecuter = commandExecuter;
            this.localSettingHandler = localSettingHandler;
        }

        #region フィールド
        private readonly ICurrent current;

        private readonly ICommandExecuter commandExecuter;

        private readonly ILocalSettingHandler localSettingHandler;
        #endregion

        /// <summary>
        /// プレイヤーAで開く
        /// </summary>
        /// <param name="videoInfo"></param>
        /// <returns></returns>
        public IAttemptResult OpenInPlayerA(IListVideoInfo videoInfo)
        {
            var appPath = this.localSettingHandler.GetStringSetting(SettingsEnum.PlayerAPath);
            if (appPath is null) return new AttemptResult() { Message = "プレイヤーAは登録されていません。" };

            return this.OpenPlayer(appPath, videoInfo);
        }

        /// <summary>
        /// プレイヤーBで開く
        /// </summary>
        /// <param name="videoInfo"></param>
        /// <returns></returns>
        public IAttemptResult OpenInPlayerB(IListVideoInfo videoInfo)
        {
            var appPath = this.localSettingHandler.GetStringSetting(SettingsEnum.PlayerBPath);
            if (appPath is null) return new AttemptResult() { Message = "プレイヤーBは登録されていません。" };

            return this.OpenPlayer(appPath, videoInfo);
        }

        /// <summary>
        /// アプリAで開く
        /// </summary>
        /// <param name="videoInfo"></param>
        /// <returns></returns>
        public IAttemptResult SendToAppA(IListVideoInfo videoInfo)
        {
            var appPath = this.localSettingHandler.GetStringSetting(SettingsEnum.AppAPath);
            if (appPath is null) return new AttemptResult() { Message = "アプリAは登録されていません。" };

            var paramBase = this.localSettingHandler.GetStringSetting(SettingsEnum.AppAParam);
            if (paramBase is null)
            {
                paramBase = "<url>";
            }

            return this.SendToAppCommand(appPath, paramBase, videoInfo);
        }

        /// <summary>
        /// アプリBで開く
        /// </summary>
        /// <param name="videoInfo"></param>
        /// <returns></returns>
        public IAttemptResult SendToAppB(IListVideoInfo videoInfo)
        {
            var appPath = this.localSettingHandler.GetStringSetting(SettingsEnum.AppBPath);
            if (appPath is null) return new AttemptResult() { Message = "アプリBは登録されていません。" };

            var paramBase = this.localSettingHandler.GetStringSetting(SettingsEnum.AppBParam);
            if (paramBase is null)
            {
                paramBase = "<id>";
            }

            return this.SendToAppCommand(appPath, paramBase, videoInfo);
        }

        #region private
        /// <summary>
        /// プレイヤーで開く
        /// </summary>
        /// <param name="appPath"></param>
        /// <param name="videoInfo"></param>
        /// <returns></returns>
        private IAttemptResult OpenPlayer(string appPath, IListVideoInfo videoInfo)
        {
            var folderPath = this.current.SelectedPlaylist?.Folderpath;

            if (!videoInfo.IsDownloaded || videoInfo.FileName.IsNullOrEmpty()) return new AttemptResult() { Message = $"{videoInfo.NiconicoId}はダウンロードされていません。", };
            if (folderPath is null) return new AttemptResult() { Message = "フォルダーパスが設定されていません。" };

            var path = Path.Combine(folderPath, videoInfo.FileName)
                .Replace(@"\\?\", string.Empty)
                ;
            return this.commandExecuter.Execute(appPath, $"\"{path}\"");
        }

        /// <summary>
        /// アプリに送る
        /// </summary>
        /// <param name="appPath"></param>
        /// <param name="argBase"></param>
        /// <param name="videoInfo"></param>
        /// <returns></returns>
        private IAttemptResult SendToAppCommand(string appPath, string argBase, IListVideoInfo videoInfo)
        {
            var constructedArg = argBase
                .Replace("<url>", Net.NiconicoWatchUrl + videoInfo.NiconicoId)
                .Replace("<url:short>", Net.NiconicoShortUrl + videoInfo.NiconicoId)
                .Replace("<id>", videoInfo.NiconicoId)
                ;

            return this.commandExecuter.Execute(appPath, constructedArg);
        }
        #endregion
    }
}
