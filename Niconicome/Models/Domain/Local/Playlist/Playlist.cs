using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Extensions.System.List;

namespace Niconicome.Models.Domain.Local.Playlist
{

    public interface IPlaylist
    {
        /// <summary>
        /// 動画を追加
        /// </summary>
        /// <param name="video"></param>
        void Add(IVideo video);

        /// <summary>
        /// 複数の動画を追加
        /// </summary>
        /// <param name="videos"></param>
        void AddRange(IEnumerable<IVideo> videos);

        /// <summary>
        /// 動画数
        /// </summary>
        int Count { get; }

        /// <summary>
        /// プレイリスト名
        /// </summary>
        string PlaylistName { get; }

        /// <summary>
        /// 動画
        /// </summary>
        ReadOnlyCollection<IVideo> Videos { get; }
    }

    public interface IVideo
    {
        /// <summary>
        /// ファイルパス
        /// </summary>
        string Path { get; }

        /// <summary>
        /// タイトル
        /// </summary>
        string Title { get; }

        /// <summary>
        /// 投稿者名
        /// </summary>
        string OwnerName { get; }
    }

    public interface IPlaylistHandler
    {
        string CreatePlaylist(IPlaylist playlistData);
    }

    public class Playlist : IPlaylist
    {
        private readonly List<IVideo> _videos = new();

        public void Add(IVideo video)
        {
            this._videos.AddUnique(video);
        }

        public void AddRange(IEnumerable<IVideo> videos)
        {
            foreach (var path in videos)
            {
                this.Add(path);
            }
        }

        public int Count => this._videos.Count;

        public string PlaylistName { get; init; } = string.Empty;

        public ReadOnlyCollection<IVideo> Videos => this._videos.AsReadOnly();


    }

    public record Video(string Path, string Title, string OwnerName) : IVideo;

}
