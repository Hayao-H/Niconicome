using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Xml.Linq;
using Niconicome.Extensions.System.List;
using Niconicome.Models.Domain.Local.DataBackup;
using Niconicome.Models.Domain.Local.Settings;
using Niconicome.Models.Domain.Local.Store.V2;
using Niconicome.Models.Domain.Niconico.Net.Xml;
using Niconicome.Models.Helper.Result;
using Niconicome.Models.Playlist.V2.Manager;
using Niconicome.Models.Utils.Reactive;
using Error = Niconicome.Models.Domain.Utils.Error;

namespace Niconicome.Models.Local.Restore
{

    public interface IRestoreManager
    {
        /// <summary>
        /// 探索ディレクトリを追加
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        Task<IAttemptResult<int>> AddVideoDirectoryAsync(string path);

        /// <summary>
        /// 探索ディレクトリから動画を再取得
        /// </summary>
        /// <returns></returns>
        Task<IAttemptResult<int>> GetVideosFromVideoDirectoryAsync();

        /// <summary>
        /// 探索ディレクトリを削除
        /// </summary>
        /// <param name="path"></param>
        IAttemptResult DeleteVideoDirectory(string path);

        /// <summary>
        /// 全ての動画・プレイリストを削除
        /// </summary>
        IAttemptResult DeleteAllVideosAndPlaylists();

        /// <summary>
        /// 設定をリセット
        /// </summary>
        IAttemptResult ResetSettings();

        /// <summary>
        /// バックアップを適応
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        IAttemptResult ApplyBackup(string guid);

        /// <summary>
        /// バックアップを作成
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        IAttemptResult CreateBackup(string name);

        /// <summary>
        /// バックアップを削除
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        IAttemptResult RemoveBackup(string guid);

        /// <summary>
        /// 初期化
        /// </summary>
        /// <returns></returns>
        IAttemptResult Initialize();

        /// <summary>
        /// 使用されていない動画等を削除
        /// </summary>
        /// <returns></returns>
        Task<IAttemptResult> CleanDataAsync();

        /// <summary>
        /// バックアップデータ
        /// </summary>
        ReadOnlyObservableCollection<IBackupData> Backups { get; }

        /// <summary>
        /// 探索ディレクトリ
        /// </summary>
        ReadOnlyObservableCollection<string> VideoFileDirectories { get; }

        /// <summary>
        /// 動画読み込みフラグ
        /// </summary>
        IBindableProperty<bool> IsGettingVideosProcessing { get; }

        /// <summary>
        /// バックアップ適用フラグ
        /// </summary>
        IBindableProperty<bool> IsApplyingBackupProcessing { get; }
    }

    public class RestoreManager : IRestoreManager
    {

        public RestoreManager(IBackupManager backuphandler, IVideoFileStore fileStore, ISettingsContainer settingsContainer, IPlaylistStore playlistStore, IVideoStore videoStore, Error::IErrorHandler errorHandler, IPlaylistManager playlistManager, IStoreCleaner storeCleaner)
        {
            this._backuphandler = backuphandler;
            this._fileStore = fileStore;
            this._settingsContainer = settingsContainer;
            this._playlistStore = playlistStore;
            this._videoStore = videoStore;
            this._errorHandler = errorHandler;
            this._playlistManager = playlistManager;
            this._storeCleaner = storeCleaner;
            this.Backups = new ReadOnlyObservableCollection<IBackupData>(this._backups);
            this.VideoFileDirectories = new ReadOnlyObservableCollection<string>(this._videoFileDirectories);
        }
        #region field

        private readonly IBackupManager _backuphandler;

        private readonly IVideoFileStore _fileStore;

        private readonly ISettingsContainer _settingsContainer;

        private readonly IPlaylistStore _playlistStore;

        private readonly IVideoStore _videoStore;

        private readonly IPlaylistManager _playlistManager;

        private readonly IStoreCleaner _storeCleaner;

        private readonly Error::IErrorHandler _errorHandler;

        private readonly ObservableCollection<IBackupData> _backups = new();

        private readonly ObservableCollection<string> _videoFileDirectories = new();

        #endregion

        #region Props

        public ReadOnlyObservableCollection<IBackupData> Backups { get; init; }

        public ReadOnlyObservableCollection<string> VideoFileDirectories { get; init; }

        public IBindableProperty<bool> IsGettingVideosProcessing { get; init; } = new BindableProperty<bool>(false);

        public IBindableProperty<bool> IsApplyingBackupProcessing { get; init; } = new BindableProperty<bool>(false);

        #endregion

        #region Method

        public async Task<IAttemptResult<int>> AddVideoDirectoryAsync(string path)
        {
            IAttemptResult<ISettingInfo<List<string>>> sResult = this._settingsContainer.GetSetting(SettingNames.VideoSearchDirectories, new List<string>());
            if (!sResult.IsSucceeded || sResult.Data is null)
            {
                return AttemptResult<int>.Fail(sResult.Message);
            }

            if (sResult.Data.Value.Contains(path))
            {
                this._errorHandler.HandleError(RestoreManagerError.VideoDirectoryAllreadyRegistered, path);
                return AttemptResult<int>.Fail(this._errorHandler.GetMessageForResult(RestoreManagerError.VideoDirectoryAllreadyRegistered, path));
            }

            List<string> copied = sResult.Data.Value.Copy();

            copied.Add(path);
            sResult.Data.Value = copied;

            IAttemptResult<int>? result = null;

            await Task.Run(async () =>
            {
                result = await this._fileStore.AddFilesFromDirectoryListAsync(new List<string>() { path });
            });

            if (result?.IsSucceeded ?? false)
            {
                this._videoFileDirectories.Add(path);
            }

            return result ?? AttemptResult<int>.Fail();

        }

        public async Task<IAttemptResult<int>> GetVideosFromVideoDirectoryAsync()
        {
            IAttemptResult<ISettingInfo<List<string>>> sResult = this._settingsContainer.GetSetting(SettingNames.VideoSearchDirectories, new List<string>());
            if (!sResult.IsSucceeded || sResult.Data is null)
            {
                return AttemptResult<int>.Fail(sResult.Message);
            }


            IAttemptResult<int>? result = null;

            await Task.Run(async () =>
            {
                result = await this._fileStore.AddFilesFromDirectoryListAsync(sResult.Data.Value);
            });

            return result ?? AttemptResult<int>.Fail();
        }

        public IAttemptResult DeleteVideoDirectory(string path)
        {
            IAttemptResult<ISettingInfo<List<string>>> sResult = this._settingsContainer.GetSetting(SettingNames.VideoSearchDirectories, new List<string>());
            if (!sResult.IsSucceeded || sResult.Data is null)
            {
                return AttemptResult<int>.Fail(sResult.Message);
            }

            if (!sResult.Data.Value.Contains(path))
            {
                this._errorHandler.HandleError(RestoreManagerError.VideoDirectoryNotRegistered, path);
                return AttemptResult<int>.Fail(this._errorHandler.GetMessageForResult(RestoreManagerError.VideoDirectoryNotRegistered, path));
            }

            List<string> copied = sResult.Data.Value.Copy();

            copied.Remove(path);
            sResult.Data.Value = copied;

            this._videoFileDirectories.Remove(path);

            return AttemptResult.Succeeded();
        }

        public IAttemptResult<ReadOnlyCollection<string>> GetAllVideoDirectories()
        {

            IAttemptResult<ISettingInfo<List<string>>> sResult = this._settingsContainer.GetSetting(SettingNames.VideoSearchDirectories, new List<string>());
            if (!sResult.IsSucceeded || sResult.Data is null)
            {
                return AttemptResult<ReadOnlyCollection<string>>.Fail(sResult.Message);
            }

            return AttemptResult<ReadOnlyCollection<string>>.Succeeded(sResult.Data.Value.AsReadOnly());
        }

        public IAttemptResult DeleteAllVideosAndPlaylists()
        {
            IAttemptResult playlistResult = this._playlistStore.Clear();
            if (!playlistResult.IsSucceeded)
            {
                return playlistResult;
            }

            IAttemptResult videoResult = this._videoStore.Clear();
            if (!videoResult.IsSucceeded)
            {
                return videoResult;
            }

            this._videoStore.Flush();
            this._playlistManager.Initialize();

            return AttemptResult.Succeeded();
        }

        public IAttemptResult ResetSettings()
        {
            return this._settingsContainer.ClearSettings();
        }

        public IAttemptResult ApplyBackup(string guid)
        {
            IAttemptResult result = this._backuphandler.ApplyBackup(guid);
            if (!result.IsSucceeded)
            {
                return result;
            }

            this._videoStore.Flush();
            this._playlistManager.Initialize();

            return AttemptResult.Succeeded();
        }

        public IAttemptResult<IEnumerable<IBackupData>> GetAllBackups()
        {
            return this._backuphandler.GetAllBackups();
        }

        public IAttemptResult CreateBackup(string name)
        {
            IAttemptResult<IBackupData> result = this._backuphandler.CreateBackup(name);
            if (!result.IsSucceeded || result.Data is null)
            {
                return AttemptResult.Fail(result.Message);
            }

            this._backups.Add(result.Data);

            return AttemptResult.Succeeded();
        }

        public IAttemptResult RemoveBackup(string guid)
        {
            IAttemptResult result = this._backuphandler.RemoveBackup(guid);
            if (!result.IsSucceeded)
            {
                return AttemptResult.Fail(result.Message);
            }

            this._backups.RemoveAll(b => b.GUID == guid);

            return AttemptResult.Succeeded();
        }

        public IAttemptResult Initialize()
        {
            this._backups.Clear();
            this._videoFileDirectories.Clear();

            IAttemptResult<IEnumerable<IBackupData>> bResult = this.GetAllBackups();
            if (!bResult.IsSucceeded || bResult.Data is null)
            {
                return AttemptResult.Fail(bResult.Message);
            }

            this._backups.Addrange(bResult.Data);

            IAttemptResult<ReadOnlyCollection<string>> vResult = this.GetAllVideoDirectories();
            if (!vResult.IsSucceeded || vResult.Data is null)
            {
                return AttemptResult.Fail(vResult.Message);
            }
            this._videoFileDirectories.Addrange(vResult.Data);

            return AttemptResult.Succeeded();
        }

        public async Task<IAttemptResult> CleanDataAsync()
        {
            IAttemptResult pResult = await Task.Run(() => this._storeCleaner.CleanPlaylists());
            if (pResult.IsSucceeded)
            {
                return pResult;
            }

            return await Task.Run(() => this._storeCleaner.CleanVideos());
        }


        #endregion

    }
}
