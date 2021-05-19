using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VList = Niconicome.Models.Playlist.VideoList;
using Reactive.Bindings;
using Niconicome.Models.Playlist.Playlist;

namespace NiconicomeTest.Stabs.Models.Playlist.VideoList
{
    class CurrentStab : VList::ICurrent
    {
        public ReactiveProperty<ITreePlaylistInfo?> SelectedPlaylist { get; init; } = new();

        public int PrevSelectedPlaylistID { get; set; }


        public event EventHandler? SelectedPlaylistChanged;
    }
}
