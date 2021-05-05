using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.ViewModels;
using Reactive.Bindings;

namespace Niconicome.Models.Playlist.VideoList
{
    public interface ICurrent
    {
        ReactiveProperty<ITreePlaylistInfo?> SelectedPlaylist { get; }
        int PrevSelectedPlaylistID { get; }
    }

    public class Current : BindableBase, ICurrent
    {
        public Current()
        {
            this.SelectedPlaylist = new ReactiveProperty<ITreePlaylistInfo?>();
            this.SelectedPlaylist.Zip(this.SelectedPlaylist.Skip(1), (x, y) => new { OldValue = x, NewValue = y })
                .Subscribe(v =>
                {
                    if (v.OldValue is null) return;
                    this.PrevSelectedPlaylistID = v.OldValue.Id;
                });
        }

        /// <summary>
        /// 現在選択されているプレイリストのID
        /// </summary>
        public ReactiveProperty<ITreePlaylistInfo?> SelectedPlaylist { get; init; }

        /// <summary>
        /// ひとつまえに選択していたプレイリスト
        /// </summary>
        public int PrevSelectedPlaylistID { get; private set; }

    }
}
