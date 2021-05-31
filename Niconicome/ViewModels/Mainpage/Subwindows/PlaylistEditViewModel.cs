using System;
using System.IO;
using System.Reflection;
using System.Windows;
using Niconicome.Models.Playlist;
using WS=Niconicome.Workspaces;
using MsApi = Microsoft.WindowsAPICodePack;
using Niconicome.Models.Playlist.Playlist;

namespace Niconicome.ViewModels.Mainpage.Subwindows
{
    /// <summary>
    /// プレイリスト情報更新
    /// </summary>
    class PlaylistEditViewModel : BindableBase
    {
        public PlaylistEditViewModel() : this(WS::Mainpage.CurrentPlaylist.SelectedPlaylist.Value ?? new BindableTreePlaylistInfo())
        {
        }

        public PlaylistEditViewModel(ITreePlaylistInfo playlist)
        {
            this.playlist = playlist; 
            this.PlaylistName = this.playlist.Name ?? string.Empty;
            this.FolderPath = this.playlist.Folderpath ?? string.Empty;

            this.OnExit = new CommandBase<Window>(arg => true, arg =>
            {
                var oldPlaylist =this.playlist;
                if (oldPlaylist is not null && (oldPlaylist.Name != this.PlaylistName || oldPlaylist.Folderpath != this.FolderPath))
                {
                    var newPlaylist = oldPlaylist.Clone();
                    newPlaylist.Name = this.PlaylistName;
                    newPlaylist.Folderpath = this.FolderPath;
                    WS::Mainpage.PlaylistHandler.Update(newPlaylist);
                    WS::Mainpage.CurrentPlaylist.SelectedPlaylist.Value = newPlaylist;
                }

                if (arg is not null && arg is Window window) window.Close();
            });

            this.SelectFolder = new CommandBase<Window>(_ => true, _ =>
            {
                var dialog = new MsApi::Dialogs.CommonOpenFileDialog
                {
                    Title = "フォルダーを選択してください",
                    IsFolderPicker = true,
                    RestoreDirectory = true,
                    InitialDirectory = AppContext.BaseDirectory
                };

                var result = dialog.ShowDialog();

                if (result == MsApi::Dialogs.CommonFileDialogResult.Ok)
                {
                    this.FolderPath = dialog.FileName;
                }
            });
        }

        private string playlistNameField = string.Empty;

        private string folderPathField = string.Empty;

        private readonly ITreePlaylistInfo playlist;


        /// <summary>
        /// ウィンドウを閉じる
        /// </summary>
        public CommandBase<Window> OnExit { get; init; }


        /// <summary>
        /// 参照
        /// </summary>
        public CommandBase<Window> SelectFolder { get; init; }


        /// <summary>
        /// プレイリスト名
        /// </summary>
        public string PlaylistName
        {
            get => this.playlistNameField; set => this.SetProperty(ref this.playlistNameField, value);
        }

        /// <summary>
        /// フォルダー名
        /// </summary>
        public string FolderPath
        {
            get => this.folderPathField; set => this.SetProperty(ref this.folderPathField, value);
        }

    }

 }
