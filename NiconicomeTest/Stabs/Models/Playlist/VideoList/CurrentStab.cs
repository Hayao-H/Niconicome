using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Playlist.VideoList;

namespace NiconicomeTest.Stabs.Models.Playlist.VideoList
{
    class CurrentStab : ICurrent
    {
       public int SelectedPlaylistID { get; set; }

       public event EventHandler? SelectedPlaylistChanged;
    }
}
