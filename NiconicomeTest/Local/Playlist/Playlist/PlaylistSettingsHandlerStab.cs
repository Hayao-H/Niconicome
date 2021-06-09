using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Local.Settings;
using Niconicome.Models.Playlist.Playlist;

namespace NiconicomeTest.Local.Playlist.Playlist
{
    class PlaylistSettingsHandlerStab : IPlaylistSettingsHandler
    {
        public bool IsDownloadFailedHistoryDisabled => false;

        public bool IsDownloadSucceededHistoryDisabled => false;
    }
}
