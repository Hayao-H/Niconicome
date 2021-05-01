using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niconicome.Models.Playlist.VideoList
{
    public interface ICurrent
    {
        ITreePlaylistInfo? SelectedPlaylist { get; set; }
        int PrevSelectedPlaylistID { get; }
        event EventHandler? SelectedPlaylistChanged;
    }

    public class Current : ICurrent
    {

        /// <summary>
        /// 現在選択されているプレイリストのID
        /// </summary>
        public ITreePlaylistInfo? SelectedPlaylist
        {
            get => this.selectedPlaylistfield;
            set
            {
                this.PrevSelectedPlaylistID = this.selectedPlaylistfield?.Id ?? -1;
                this.selectedPlaylistfield = value;
                this.RaiseSelectedPlaylistChanged();
            }
        }

        /// <summary>
        /// ひとつまえに選択していたプレイリスト
        /// </summary>
        public int PrevSelectedPlaylistID { get; private set; }


        /// <summary>
        /// プレイリスト変更イベント
        /// </summary>
        public event EventHandler? SelectedPlaylistChanged;

        private ITreePlaylistInfo? selectedPlaylistfield;

        private void RaiseSelectedPlaylistChanged()
        {
            this.SelectedPlaylistChanged?.Invoke(this, EventArgs.Empty);
        }

    }
}
