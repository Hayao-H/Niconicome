using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Extensions.System;
using Niconicome.Models.Const;
using Niconicome.Models.Domain.Local.IO;
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
        ReadOnlyReactiveProperty<bool> IsTemporaryPlaylist { get; }
        string PlaylistFolderPath { get; }
        int PrevSelectedPlaylistID { get; }
    }

    public class Current : BindableBase, ICurrent
    {
        public Current(ILocalSettingHandler settingHandler,IPlaylistTreeHandler treeHandler)
        {
            this.settingHandler = settingHandler;
            this.treeHandler = treeHandler;

            this.SelectedPlaylist = new ReactiveProperty<ITreePlaylistInfo?>();
            this.IsTemporaryPlaylist = this.SelectedPlaylist
                .Select(p => p?.IsTemporary ?? false)
                .ToReadOnlyReactiveProperty();

            this.SelectedPlaylist.Subscribe(value =>
            {
                if (value is not null)
                {
                    string? defaultFolder = this.settingHandler.GetStringSetting(SettingsEnum.DefaultFolder);

                    if (string.IsNullOrEmpty(defaultFolder))
                    {
                        defaultFolder = FileFolder.DefaultDownloadDir;
                    } else if (defaultFolder.Contains("<autoMap>"))
                    {
                        List<string> ancesnter = this.treeHandler.GetListOfAncestor(value.Id);
                        if (ancesnter.Count > 1)
                        {
                            ancesnter = ancesnter.GetRange(1, ancesnter.Count - 1);
                        }
                        string path = string.Join('\\', ancesnter);
                        defaultFolder = defaultFolder.Replace("<autoMap>", path);
                    }

                    this.PlaylistFolderPath = value.Folderpath.IsNullOrEmpty() ?  defaultFolder : value.Folderpath;
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

        private readonly IPlaylistTreeHandler treeHandler;

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
        /// 一時プレイリストフラグ
        /// </summary>
        public ReadOnlyReactiveProperty<bool> IsTemporaryPlaylist { get; init; }


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
