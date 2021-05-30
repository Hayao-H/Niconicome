using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Extensions.System;
using Niconicome.Models.Const;
using Niconicome.Models.Local.Settings;
using Niconicome.Models.Playlist.Playlist;
using Niconicome.ViewModels;
using Reactive.Bindings;

namespace Niconicome.Models.Playlist.VideoList
{
    public interface ICurrent
    {
        ReactiveProperty<ITreePlaylistInfo?> SelectedPlaylist { get; }
        ReactiveProperty<int> CurrentSelectedIndex { get; }
        string PlaylistFolderPath { get; }
        int PrevSelectedPlaylistID { get; }
    }

    public class Current : BindableBase, ICurrent
    {
        public Current(ILocalSettingHandler settingHandler)
        {
            this.settingHandler = settingHandler;

            this.SelectedPlaylist = new ReactiveProperty<ITreePlaylistInfo?>();
            this.SelectedPlaylist.Subscribe(value =>
            {
                if (value is not null)
                {
                    this.PlaylistFolderPath = value.Folderpath.IsNullOrEmpty() ? this.settingHandler.GetStringSetting(SettingsEnum.DefaultFolder) ?? FileFolder.DefaultDownloadDir : value.Folderpath;
                }
                else
                {
                    this.PlaylistFolderPath = string.Empty;
                }
            });
            this.SelectedPlaylist.Zip(this.SelectedPlaylist.Skip(1), (x, y) => new { OldValue = x, NewValue = y })
                .Subscribe(v =>
                {
                    if (v.OldValue is null) return;
                    this.PrevSelectedPlaylistID = v.OldValue.Id;
                });
            this.CurrentSelectedIndex = new ReactiveProperty<int>();
        }

        #region DI

        private readonly ILocalSettingHandler settingHandler;

        #endregion

        /// <summary>
        /// 現在選択されているプレイリストのID
        /// </summary>
        public ReactiveProperty<ITreePlaylistInfo?> SelectedPlaylist { get; init; }

        /// <summary>
        /// 選択された動画のインデックス
        /// </summary>
        public ReactiveProperty<int> CurrentSelectedIndex { get; init; }

        /// <summary>
        /// ひとつまえに選択していたプレイリスト
        /// </summary>
        public int PrevSelectedPlaylistID { get; private set; }

        /// <summary>
        /// 保存フォルダーのパス
        /// </summary>
        public string PlaylistFolderPath { get; private set; } = string.Empty;


    }
}
