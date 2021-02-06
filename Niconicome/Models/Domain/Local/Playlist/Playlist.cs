using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Extensions.System.List;

namespace Niconicome.Models.Domain.Local.Playlist
{

    public interface IPlaylist
    {
        void Add(string filpath);
        void AddRange(IEnumerable<string> filepaths);
        void Remove(string filepath);
        void Clear();
        IEnumerable<string> GetAllFile();
        int Count { get; }
        string PlaylistName { get; }
    }

    public interface IPlaylistHandler
    {
        string CreatePlaylist(IPlaylist playlistData);
    }

    /// <summary>
    /// 全てのハンドラが共通して扱うことが出来るプレイリスト
    /// </summary>
    public class Playlist : IPlaylist
    {
        private readonly List<string> filePaths = new();

        /// <summary>
        /// ファイルを追加
        /// </summary>
        /// <param name="filpath"></param>
        public void Add(string filpath)
        {
            this.filePaths.AddUnique(filpath);
        }

        /// <summary>
        /// 複数のファイルを追加
        /// </summary>
        /// <param name="filepaths"></param>
        public void AddRange(IEnumerable<string> filepaths)
        {
            foreach (var path in filepaths)
            {
                this.Add(path);
            }
        }


        /// <summary>
        /// ファイルを削除
        /// </summary>
        /// <param name="filepath"></param>
        public void Remove(string filepath)
        {
            this.filePaths.Remove(filepath);
        }

        /// <summary>
        /// ファイルを全て削除
        /// </summary>
        public void Clear()
        {
            this.filePaths.Clear();
        }

        /// <summary>
        /// ファイルパスを取得
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetAllFile()
        {
            return this.filePaths;
        }

        /// <summary>
        /// ファイル数
        /// </summary>
        public int Count { get => this.filePaths.Count; }

        /// <summary>
        /// プレイリスト名
        /// </summary>
        public string PlaylistName { get; set; } = string.Empty;

    }
}
