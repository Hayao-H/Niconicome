using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WS = Niconicome.Workspaces;
using MsApi = Microsoft.WindowsAPICodePack;
using System.Windows;
using Niconicome.Extensions.System;
using Niconicome.Models.Playlist;
using Niconicome.Models.Local.External.Import;
using System.Threading;

namespace Niconicome.ViewModels.Setting.Pages
{
    class ImportPageViewModel : BindableBase
    {
        public ImportPageViewModel()
        {
            this.SetXenoPathCommand = new CommandBase<object>(_ => !this.isFetching, _ =>
            {
                var dialog = new MsApi::Dialogs.CommonOpenFileDialog
                {
                    Title = "「設定：固定URL.txt」を選択してください",
                    InitialDirectory = AppContext.BaseDirectory,
                    DefaultExtension = ".txt"
                };

                var result = dialog.ShowDialog();

                if (result == MsApi::Dialogs.CommonFileDialogResult.Ok)
                {
                    if (!dialog.FileName.EndsWith(".txt")) return;
                    this.xenoFilePath = dialog.FileName;
                }

                this.XenoPathCheckVisibility = Visibility.Visible;
            });

            this.ImportFromXenoCommand = new CommandBase<object>(_ => !this.isFetching, async _ =>
             {
                 if (this.xenoFilePath.IsNullOrEmpty())
                 {
                     WS::SettingPage.SnackbarMessageQueue.Enqueue("Xenoの設定ファイルが選択されていません。");
                     return;
                 }
                 if (this.SelectedParent is null)
                 {
                     WS::SettingPage.SnackbarMessageQueue.Enqueue("追加先プレイリストが選択されていません。");
                     return;
                 }

                 this.message.Clear();
                 this.OnPropertyChanged(nameof(this.Message));
                 this.Message = "インポートを開始します。";
                 this.StartFetch();
                 this.XenoImportProgressVisibility = Visibility.Visible;
                 this.importCTS = new CancellationTokenSource();
                 WS::SettingPage.State.IsImportingFromXeno = true;

                 var setting = new XenoImportSettings()
                 {
                     AddDirectly = true,
                     TargetPlaylistId = this.SelectedParent.Id,
                     DataFilePath = this.xenoFilePath
                 };

                 IXenoImportTaskResult result = await WS::SettingPage.XenoImportManager.InportFromXeno(setting, m => this.Message = m, this.importCTS.Token);

                 this.Message = $"インポートに成功したプレイリストの数; {result.SucceededPaylistCount}";
                 if (result.FailedPlaylistCount > 0)
                 {
                     this.Message = $"インポートに失敗したプレイリストの数; {result.FailedPlaylistCount}";
                 }

                 this.Message = $"インポートに成功した動画の数; {result.SucceededVideoCount}";
                 if (result.FailedVideoCount > 0)
                 {
                     this.Message = $"インポートに失敗した動画の数; {result.FailedPlaylistCount}";
                 }

                 this.CompleteFetch();
                 this.XenoImportProgressVisibility = Visibility.Hidden;
                 WS::SettingPage.State.IsImportingFromXeno = false;

             });

            this.ImportCancellCommand = new CommandBase<object>(_ => this.isFetching, _ =>
            {
                this.importCTS?.Cancel();
                this.importCTS = null;
                this.CompleteFetch();
                this.XenoImportProgressVisibility = Visibility.Hidden;
                WS::SettingPage.State.IsImportingFromXeno = false;
            });

            this.SelectedParent = WS::SettingPage.PlaylistTreeHandler.GetRootPlaylist();

            this.SelectablePlaylists = new List<ITreePlaylistInfo>();
            var playlists = WS::SettingPage.PlaylistTreeHandler.GetAllPlaylists().Where(p => !p.IsConcrete);
            this.SelectablePlaylists.AddRange(playlists);
        }

        private Visibility xenoImportProgressVisibility = Visibility.Hidden;

        private Visibility xenoPathCheckVisibility = Visibility.Hidden;

        private ITreePlaylistInfo? selectedParent;

        private readonly StringBuilder message = new();

        private string xenoFilePath = string.Empty;

        private CancellationTokenSource? importCTS;

        private bool isFetching;

        /// <summary>
        /// プログレスバーの可視性
        /// </summary>
        public Visibility XenoImportProgressVisibility { get => this.xenoImportProgressVisibility; set => this.SetProperty(ref this.xenoImportProgressVisibility, value); }

        /// <summary>
        /// チェックマークの可視性
        /// </summary>
        public Visibility XenoPathCheckVisibility { get => this.xenoPathCheckVisibility; set => this.SetProperty(ref this.xenoPathCheckVisibility, value); }

        /// <summary>
        /// 選択されたプレイリスト
        /// </summary>
        public ITreePlaylistInfo? SelectedParent { get => this.selectedParent; set => this.SetProperty(ref this.selectedParent, value); }

        /// <summary>
        /// メッセージ
        /// </summary>
        public string Message
        {
            get => this.message.ToString(); set
            {
                this.message.AppendLine(value);
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// インポートコマンド(Xeno)
        /// </summary>
        public CommandBase<object> ImportFromXenoCommand { get; init; }

        /// <summary>
        /// Xeno設定コマンド
        /// </summary>
        public CommandBase<object> SetXenoPathCommand { get; init; }

        /// <summary>
        /// インポートをキャンセル
        /// </summary>
        public CommandBase<object> ImportCancellCommand { get; init; }

        /// <summary>
        /// 選択可能なプレイリスト
        /// </summary>
        public List<ITreePlaylistInfo> SelectablePlaylists { get; init; }

        private void StartFetch()
        {
            this.isFetching = true;
            this.RefreshCommand();
        }

        private void CompleteFetch()
        {
            this.isFetching = false;
            this.RefreshCommand();
        }

        private void RefreshCommand()
        {
            this.SetXenoPathCommand.RaiseCanExecutechanged();
            this.ImportFromXenoCommand.RaiseCanExecutechanged();
            this.ImportCancellCommand.RaiseCanExecutechanged();
        }
    }
}
