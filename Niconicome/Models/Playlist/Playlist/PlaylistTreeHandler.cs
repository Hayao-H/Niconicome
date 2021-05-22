using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Extensions.System.List;

namespace Niconicome.Models.Playlist.Playlist
{

    public interface IPlaylistTreeHandler
    {
        ITreePlaylistInfo? GetPlaylist(int id);
        void Remove(int id);
        void Merge(ITreePlaylistInfo playlist, bool noAssociate = false);
        void MergeRange(IEnumerable<ITreePlaylistInfo> playlists, bool noAssociate = false);
        void Initialize(IEnumerable<ITreePlaylistInfo> playlists);
        void Clear();
        bool Contains(int id);
        bool IsLastChild(ITreePlaylistInfo child);
        bool IsLastChild(int id);
        ITreePlaylistInfo? GetParent(int id);
        IEnumerable<ITreePlaylistInfo> GetAllPlaylists();
        ObservableCollection<ITreePlaylistInfo> Playlists { get; }
    }

    /// <summary>
    /// TreePlaylistInfoをよしなにしてくれる
    /// (実際にはインスタンスを管理している)
    /// </summary>
    public class PlaylistTreeHandler : IPlaylistTreeHandler
    {
        private List<ITreePlaylistInfo> innertPlaylists { get; init; } = new();

        public ObservableCollection<ITreePlaylistInfo> Playlists { get; init; } = new();

        /// <summary>
        /// 指定したIDのプレイリストを取得する
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ITreePlaylistInfo? GetPlaylist(int id)
        {
            return this.innertPlaylists.FirstOrDefault(p => p.Id == id);
        }

        /// <summary>
        /// 指定したIDのプレイリストを削除する
        /// </summary>
        /// <param name="id"></param>
        public void Remove(int id)
        {
            var parent = this.GetParent(id);
            if (parent is not null)
            {
                parent.Children.RemoveAll(p => p.Id == id);
            }
            this.innertPlaylists.RemoveAll(pl => pl.Id == id);

        }

        /// <summary>
        /// プレイリストをクリアする
        /// </summary>
        public void Clear()
        {
            this.innertPlaylists.Clear();
        }

        /// <summary>
        /// 指定されたIDのプレイリストを含むかどうかを返す
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool Contains(int id)
        {
            return this.innertPlaylists.Any(p => p.Id == id);
        }

        /// <summary>
        /// 最後の子プレイリストであるかどうかを判断する
        /// </summary>
        /// <param name="child"></param>
        /// <returns></returns>
        public bool IsLastChild(ITreePlaylistInfo child)
        {
            return this.IsLastChild(child.Id);
        }

        /// <summary>
        /// 最後の子プレイリストであるかどうかを判断する
        /// </summary>
        /// <param name="child"></param>
        /// <returns></returns>
        public bool IsLastChild(int id)
        {
            var parent = this.GetParent(id);
            //nullチェック
            if (parent == null) return false;

            if (parent.ChildrensIds.Count == 0) return false;

            return parent.ChildrensIds.Last() == id;
        }

        /// <summary>
        /// 開閉状態等を保持して追加
        /// </summary>
        /// <param name="playlist"></param>
        public void Merge(ITreePlaylistInfo after, bool noAssociate = false)
        {

            //重複があった場合はプロパティーを引き継いでから削除する
            if (this.Contains(after.Id))
            {
                ITreePlaylistInfo? before = this.GetPlaylist(after.Id);
                if (before != null)
                {
                    after.BeforeSeparatorVisibility = before.BeforeSeparatorVisibility;
                    after.AfterSeparatorVisibility = before.AfterSeparatorVisibility;
                    after.IsExpanded = before.IsExpanded;
                    this.Remove(before.Id);
                }
            }

            if (!noAssociate)
            {
                var parent = this.GetPlaylist(after.ParentId);
                if (parent is not null)
                {
                    if (parent.Children.Any(p => p.Id == after.Id)) parent.Children.RemoveAll(p => p.Id == after.Id);
                    parent.Children.Add(after);
                }
            }

            this.innertPlaylists.Add(after);
        }

        /// <summary>
        /// プレイリストを一括でマージする
        /// </summary>
        /// <param name="playlists"></param>
        public void MergeRange(IEnumerable<ITreePlaylistInfo> playlists, bool noAssociate = false)
        {
            foreach (var playlist in playlists)
            {
                this.Merge(playlist, noAssociate);
            }
        }

        /// <summary>
        /// プレイリストを空にして追加する
        /// </summary>
        /// <param name="playlists"></param>
        public void Initialize(IEnumerable<ITreePlaylistInfo> playlists)
        {
            this.MergeRange(playlists, true);

            var tree = this.GetTree();
            this.Playlists.Clear();
            this.Playlists.Add(tree);
        }


        /// <summary>
        /// 親プレイリストを取得する
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ITreePlaylistInfo? GetParent(int id)
        {
            ITreePlaylistInfo? self = this.GetPlaylist(id);
            if (self == null || self.ParentId == default)
            {
                return null;
            }

            int parentId = self.ParentId;
            return this.GetPlaylist(parentId);
        }

        /// <summary>
        /// ルートプレイリストを取得する
        /// </summary>
        /// <returns></returns>
        public ITreePlaylistInfo GetRoot()
        {
            return this.innertPlaylists.First(p => p.IsRoot);
        }

        /// <summary>
        /// ツリーを取得する
        /// </summary>
        /// <returns></returns>
        public ITreePlaylistInfo GetTree()
        {
            var root = this.innertPlaylists.FirstOrDefault(pl => pl.IsRoot);
            //ルートプレイリストがnullの場合はエラーを返す
            if (root == null) throw new InvalidOperationException("ルートプレイリストが存在しません。");

            return this.ConstructPlaylistInfo(root.Id);

        }

        /// <summary>
        /// すべてのプレイリストを取得する
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ITreePlaylistInfo> GetAllPlaylists()
        {
            return this.innertPlaylists;
        }


        /// <summary>
        /// 完全なプレイリストツリーを構築する
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private ITreePlaylistInfo ConstructPlaylistInfo(int id)
        {
            var playlist = this.GetPlaylist(id);

            //ルートプレイリストがnullの場合はエラーを返す
            if (playlist == null) throw new InvalidOperationException($"指定されたプレイリスト(id:{id})が存在しません。");

            //子プレイリストを構築する
            if (playlist.ChildrensIds.Count > 0)
            {
                var tmp = new List<ITreePlaylistInfo>();
                foreach (var cid in playlist.ChildrensIds)
                {
                    tmp.Add(this.ConstructPlaylistInfo(cid));
                }

                playlist.Children.Addrange(tmp.OrderBy(p => p.Id));
            }

            return playlist;
        }
    }
}
