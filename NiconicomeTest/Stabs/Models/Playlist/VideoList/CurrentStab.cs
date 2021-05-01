using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VList = Niconicome.Models.Playlist.VideoList;
using Niconicome.Models.Playlist;

namespace NiconicomeTest.Stabs.Models.Playlist.VideoList
{
    class CurrentStab : VList::ICurrent
    {
        public ITreePlaylistInfo? SelectedPlaylist { get; set; }

        public int PrevSelectedPlaylistID { get; set; }


        public event EventHandler? SelectedPlaylistChanged;
    }
}
