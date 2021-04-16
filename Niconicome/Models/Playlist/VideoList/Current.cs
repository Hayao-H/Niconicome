using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niconicome.Models.Playlist.VideoList
{
    public interface ICurrent
    {
        int SelectedPlaylistID { get; set; }
        event EventHandler? SelectedPlaylistChanged;
    }

    public class Current : ICurrent
    {

        /// <summary>
        /// 現在選択されているプレイリストのID
        /// </summary>
        public int SelectedPlaylistID
        {
            get => this.selectedPlaylistIDfield;
            set
            {
                this.selectedPlaylistIDfield = value;
                this.RaiseSelectedPlaylistChanged();
            }
        }

        /// <summary>
        /// プレイリスト変更イベント
        /// </summary>
        public event EventHandler? SelectedPlaylistChanged;

        private int selectedPlaylistIDfield;

        private void RaiseSelectedPlaylistChanged()
        {
            this.SelectedPlaylistChanged?.Invoke(this, EventArgs.Empty);
        }

    }
}
