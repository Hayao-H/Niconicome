using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Local.Server.API.Watch.V1.Error;
using Niconicome.Models.Domain.Local.Server.API.Watch.V1.LocalFile;
using Niconicome.Models.Domain.Local.Server.Core;
using Niconicome.Models.Domain.Playlist;
using Niconicome.Models.Domain.Utils.Error;
using Niconicome.Models.Helper.Result;
using Niconicome.Models.Playlist;

namespace Niconicome.Models.Domain.Local.Server.API.Watch.V1.HLS
{
    public interface IPlaylistCreator
    {
        /// <summary>
        /// プレイリストを生成する
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="sessionID"></param>
        /// <param name="port"></param>
        /// <param name="ip"></param>
        /// <returns></returns>
        HLSPlaylist GetPlaylist(IStreamInfo stream, string sessionID, int port);
    }

    internal class PlaylistCreator : IPlaylistCreator
    {

        public HLSPlaylist GetPlaylist(IStreamInfo stream, string sessionID, int port)
        {

            string videoPlaylist = this.GetVideoPlaylist(stream, sessionID, port);
            string audioPlaylist = this.GetAudioPlaylist(stream, sessionID, port);
            string masterPlaylist = this.GetMasterPlaylist(sessionID, port, stream.VideoBandWidth);

            return new HLSPlaylist(masterPlaylist, videoPlaylist, audioPlaylist, stream.VideoKey, stream.AudioKey);

        }

        private string GetVideoPlaylist(IStreamInfo stream, string sessionID, int port)
        {

            var builder = new StringBuilder();
            builder.AppendLine("#EXTM3U");
            builder.AppendLine("#EXT-X-VERSION:3");
            builder.AppendLine("#EXT-X-TARGETDURATION:6");
            builder.AppendLine("#EXT-X-MEDIA-SEQUENCE:1");
            builder.AppendLine("#EXT-X-PLAYLIST-TYPE:VOD");
            builder.AppendLine($"#EXT-X-MAP:URI=\"http://localhost:{port}/niconicome/watch/v1/{sessionID}/video/init.cmfv\"");
            foreach (var segment in stream.VideoSegments)
            {
                builder.AppendLine($"#EXTINF:{segment.Duration},");
                builder.AppendLine($"http://localhost:{port}/niconicome/watch/v1/{sessionID}/video/{segment.FileName}");
            }
            builder.AppendLine("#EXT-X-ENDLIST");

            return builder.ToString();
        }

        private string GetAudioPlaylist(IStreamInfo stream, string sessionID, int port)
        {

            var builder = new StringBuilder();
            builder.AppendLine("#EXTM3U");
            builder.AppendLine("#EXT-X-VERSION:3");
            builder.AppendLine("#EXT-X-TARGETDURATION:6");
            builder.AppendLine("#EXT-X-MEDIA-SEQUENCE:1");
            builder.AppendLine("#EXT-X-PLAYLIST-TYPE:VOD");
            builder.AppendLine($"#EXT-X-MAP:URI=\"http://localhost:{port}/niconicome/watch/v1/{sessionID}/audio/init.cmfa\"");
            foreach (var segment in stream.AudioSegments)
            {
                builder.AppendLine($"#EXTINF:{segment.Duration},");
                builder.AppendLine($"http://localhost:{port}/niconicome/watch/v1/{sessionID}/audio/{segment.FileName}");
            }
            builder.AppendLine("#EXT-X-ENDLIST");

            return builder.ToString();
        }

        private string GetMasterPlaylist(string sessionID, int port, int bandWidth)
        {
            var builder = new StringBuilder();
            builder.AppendLine("#EXTM3U");
            builder.AppendLine("#EXT-X-VERSION:3");
            builder.AppendLine("#EXT-X-INDEPENDENT-SEGMENTS");
            builder.AppendLine($"#EXT-X-MEDIA:TYPE=AUDIO,GROUP-ID=\"main-adio\",NAME=\"Main Audio\",DEFAULT=YES,URI=\"http://localhost:{port}/niconicome/watch/v1/{sessionID}/audio/playlist.m3u8\"");
            builder.AppendLine($"#EXT-X-STREAM-INF:BANDWIDTH={bandWidth},AUDIO=\"main-audio\"");
            builder.AppendLine($"http://localhost:{port}/niconicome/watch/v1/{sessionID}/video/playlist.m3u8");
            builder.AppendLine("#EXT-X-ENDLIST");

            return builder.ToString();
        }

    }

    public record HLSPlaylist(string Master, string Video, string Audio, string videoKey, string audioKey);
}
