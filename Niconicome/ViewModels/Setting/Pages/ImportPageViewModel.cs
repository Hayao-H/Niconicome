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
using Reactive.Bindings;
using System.Reactive.Linq;
using Reactive.Bindings.Extensions;
using Niconicome.Models.Playlist.Playlist;

namespace Niconicome.ViewModels.Setting.Pages
{
    class ImportPageViewModel : BindableBase
    {
        public ImportPageViewModel()
        {
            this.Message = WS::SettingPage.XenoImportManager.Message.ToReadOnlyReactiveProperty().AddTo(this.disposables);
            this.XenoPathCheckVisibility = new ReactivePropertySlim<Visibility>(Visibility.Hidden);
            this.XenoImportProgressVisibility = WS::SettingPage.XenoImportManager.IsProcessing
                .Select(v => v ? Visibility.Visible : Visibility.Hidden)
                .ToReadOnlyReactivePropertySlim();
            this.SelectedParent = new ReactivePropertySlim<ITreePlaylistInfo?>(WS::SettingPage.PlaylistTreeHandler.GetRootPlaylist());

            this.SelectablePlaylists = new List<ITreePlaylistInfo>();
            var playlists = WS::SettingPage.PlaylistTreeHandler.GetAllPlaylists().Where(p => !p.IsConcrete);
            this.SelectablePlaylists.AddRange(playlists);

            #region コマンドの初期化
            this.SetXenoPathCommand = WS::SettingPage.XenoImportManager.IsProcessing
    .Select(v => !v)
    .ToReactiveCommand()
    .WithSubscribe(() =>
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

        this.XenoPathCheckVisibility.Value = Visibility.Visible;
    });

            this.ImportFromXenoCommand = WS::SettingPage.XenoImportManager.IsProcessing
                .Select(v => !v)
                .ToAsyncReactiveCommand()
                .WithSubscribe(
                async () =>
                {
                    if (this.xenoFilePath.IsNullOrEmpty())
                    {
                        WS::SettingPage.SnackbarMessageQueue.Enqueue("Xenoの設定ファイルが選択されていません。");
                        return;
                    }
                    if (this.SelectedParent.Value is null)
                    {
                        WS::SettingPage.SnackbarMessageQueue.Enqueue("追加先プレイリストが選択されていません。");
                        return;
                    }

                    this.message.Clear();
                    this.OnPropertyChanged(nameof(this.Message));
                    WS::SettingPage.XenoImportManager.AppendMessage("インポートを開始します。");
                    WS::SettingPage.State.IsImportingFromXeno = true;

                    var setting = new XenoImportSettings()
                    {
                        AddDirectly = true,
                        TargetPlaylistId = this.SelectedParent.Value.Id,
                        DataFilePath = this.xenoFilePath
                    };

                    IXenoImportTaskResult result = await WS::SettingPage.XenoImportManager.InportFromXeno(setting);

                    WS::SettingPage.XenoImportManager.AppendMessage($"インポートに成功したプレイリストの数; {result.SucceededPaylistCount}");
                    if (result.FailedPlaylistCount > 0)
                    {
                        WS::SettingPage.XenoImportManager.AppendMessage($"インポートに失敗したプレイリストの数; {result.FailedPlaylistCount}");
                    }

                    WS::SettingPage.XenoImportManager.AppendMessage($"インポートに成功した動画の数; {result.SucceededVideoCount}");
                    if (result.FailedVideoCount > 0)
                    {
                        WS::SettingPage.XenoImportManager.AppendMessage($"インポートに失敗した動画の数; {result.FailedPlaylistCount}");
                    }

                    WS::SettingPage.State.IsImportingFromXeno = false;

                });

            this.ImportCancellCommand = WS::SettingPage.XenoImportManager.IsProcessing
                .ToReactiveCommand()
                .WithSubscribe(
                () =>
                {
                    WS::SettingPage.State.IsImportingFromXeno = false;
                    WS::SettingPage.XenoImportManager.Cancel();
                });
            #endregion
        }

        private readonly StringBuilder message = new();

        private string xenoFilePath = string.Empty;

        /// <summary>
        /// プログレスバーの可視性
        /// </summary>
        public ReadOnlyReactivePropertySlim<Visibility> XenoImportProgressVisibility { get; init; }

        /// <summary>
        /// チェックマークの可視性
        /// </summary>
        public ReactivePropertySlim<Visibility> XenoPathCheckVisibility { get; init; }

        /// <summary>
        /// 選択されたプレイリスト
        /// </summary>
        public ReactivePropertySlim<ITreePlaylistInfo?> SelectedParent { get; init; }

        /// <summary>
        /// メッセージ
        /// </summary>
        public ReadOnlyReactiveProperty<string?> Message { get; init; }

        /// <summary>
        /// インポートコマンド(Xeno)
        /// </summary>
        public AsyncReactiveCommand ImportFromXenoCommand { get; init; }

        /// <summary>
        /// Xeno設定コマンド
        /// </summary>
        public ReactiveCommand SetXenoPathCommand { get; init; }

        /// <summary>
        /// インポートをキャンセル
        /// </summary>
        public ReactiveCommand ImportCancellCommand { get; init; }

        /// <summary>
        /// 選択可能なプレイリスト
        /// </summary>
        public List<ITreePlaylistInfo> SelectablePlaylists { get; init; }

    }

    public class ImportPageViewModelD
    {
        /// <summary>
        /// プログレスバーの可視性
        /// </summary>
        public ReactivePropertySlim<Visibility> XenoImportProgressVisibility { get; init; } = new(Visibility.Visible);

        /// <summary>
        /// チェックマークの可視性
        /// </summary>
        public ReactivePropertySlim<Visibility> XenoPathCheckVisibility { get; init; } = new(Visibility.Visible);

        /// <summary>
        /// 選択されたプレイリスト
        /// </summary>
        public ReactivePropertySlim<ITreePlaylistInfo?> SelectedParent { get; init; } = new();

        /// <summary>
        /// メッセージ
        /// </summary>
        public ReactiveProperty<string> Message { get; init; } = new ReactiveProperty<string>("Hello World!!");

        /// <summary>
        /// インポートコマンド(Xeno)
        /// </summary>
        public AsyncReactiveCommand ImportFromXenoCommand { get; init; } = new();

        /// <summary>
        /// Xeno設定コマンド
        /// </summary>
        public ReactiveCommand SetXenoPathCommand { get; init; } = new();

        /// <summary>
        /// インポートをキャンセル
        /// </summary>
        public ReactiveCommand ImportCancellCommand { get; init; } = new();

        /// <summary>
        /// 選択可能なプレイリスト
        /// </summary>
        public List<ITreePlaylistInfo> SelectablePlaylists { get; init; } = new();
    }
}
