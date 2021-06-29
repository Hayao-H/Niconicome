using System.Linq;
using Niconicome.Extensions.System.List;
using Niconicome.Models.Playlist;
using Niconicome.Models.Playlist.Playlist;
using Reactive.Bindings;

namespace Niconicome.Models.Domain.Local.External.Import.Xeno
{
    public interface IXenoPlaylistConverter
    {
        ITreePlaylistInfo ConvertToTreePlaylistInfo(IXenoPlaylist playlist, bool recurse = true);
    }

    public class XenoPlaylistConverter : IXenoPlaylistConverter
    {
        public XenoPlaylistConverter(IVideoInfoContainer videoInfoContainer)
        {
            this.videoInfoContainer = videoInfoContainer;
        }

        #region DI

        private readonly IVideoInfoContainer videoInfoContainer;

        #endregion

        /// <summary>
        /// Xenoのプレイリストをアプリケーション側で使えるデータに変換する
        /// </summary>
        /// <param name="playlist"></param>
        /// <param name="recurse"></param>
        /// <returns></returns>
        public ITreePlaylistInfo ConvertToTreePlaylistInfo(IXenoPlaylist playlist, bool recurse = true)
        {
            var converted = new NonBindableTreePlaylistInfo()
            {
                Name = new ReactiveProperty<string>(playlist.Name),
                IsRemotePlaylist = playlist.IsChannel,
                RemoteType = playlist.IsChannel ? RemoteType.Channel : RemoteType.None,
                RemoteId = playlist.ChannelId??string.Empty,
            };

            converted.Videos.AddRange(playlist.Videos.Select(v => {
                var video = this.videoInfoContainer.GetVideo(v);
                return video;
            }));

            if (playlist.ChildPlaylists.Count > 0 && recurse)
            {
                converted.Children.Addrange(playlist.ChildPlaylists.Select(c => this.ConvertToTreePlaylistInfo(c, true)));
            }

            return converted;
        }
    }
}
