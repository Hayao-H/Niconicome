using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VList = Niconicome.Models.Playlist.VideoList;
using Niconicome.Models.Playlist.Playlist;
using Reactive.Bindings;
using System.Reactive.Linq;

namespace NiconicomeTest.Stabs.Models.Playlist.VideoList
{
    class CurrentStab : VList::ICurrent
    {
        public CurrentStab()
        {
            this.SelectedPlaylist = new ReactiveProperty<ITreePlaylistInfo?>();
            this.IsTemporaryPlaylist = this.SelectedPlaylist.Select(_ => true).ToReadOnlyReactiveProperty();
        }

        public ReactiveProperty<ITreePlaylistInfo?> SelectedPlaylist { get; init; }

        public int PrevSelectedPlaylistID { get; set; }
        public ReactiveProperty<int> CurrentSelectedIndex { get; init; } = new();

        public string PlaylistFolderPath { get; init; } = string.Empty;

        public ReadOnlyReactiveProperty<bool> IsTemporaryPlaylist { get; init; }


    }
}
