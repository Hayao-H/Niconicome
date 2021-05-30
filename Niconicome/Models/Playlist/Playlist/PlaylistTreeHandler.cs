using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AngleSharp.Dom;
using Niconicome.Extensions.System.List;
using Reactive.Bindings.ObjectExtensions;

namespace Niconicome.Models.Playlist.Playlist
{

    public interface IPlaylistTreeHandler
    {
        ITreePlaylistInfo? GetPlaylist(int id);
        void Remove(int id);
        void MergeRange(List<ITreePlaylistInfo> playlists, bool noAssociate = false);
        void Initialize(List<ITreePlaylistInfo> playlists);
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
        private readonly Dictionary<int, ITreePlaylistInfo> innertPlaylists = new();

        public ObservableCollection<ITreePlaylistInfo> Playlists { get; init; } = new();

        #region CRUD系メソッド

        /// <summary>
        /// 指定したIDのプレイリストを取得する
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ITreePlaylistInfo? GetPlaylist(int id)
        {
            this.innertPlaylists.TryGetValue(id, out ITreePlaylistInfo? value);
            return value;
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
            this.innertPlaylists.Remove(id);

        }

        /// <summary>
        /// プレイリストをクリアする
        /// </summary>
        public void Clear()
        {
            this.innertPlaylists.Clear();
        }

        /// <summary>
        /// プレイリストを一括でマージする
        /// </summary>
        /// <param name="playlists"></param>
        public void MergeRange(List<ITreePlaylistInfo> source, bool noAssociate = false)
        {
            for (var i = 0; i < source.Count; ++i)
            {
                ITreePlaylistInfo after = source[i];

                //重複があった場合はプロパティーを引き継いでから削除する
                ITreePlaylistInfo? before = this.GetPlaylist(after.Id);
                if (before is not null)
                {
                    after.BeforeSeparatorVisibility = before.BeforeSeparatorVisibility;
                    after.AfterSeparatorVisibility = before.AfterSeparatorVisibility;
                    after.IsExpanded = before.IsExpanded;
                    this.Remove(before.Id);
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

                this.innertPlaylists.Add(after.Id, after);

            }
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
        /// ツリーを取得する
        /// </summary>
        /// <returns></returns>
        public ITreePlaylistInfo GetTree()
        {
            var root = this.GetRoot();
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
            return this.innertPlaylists.Select(p => p.Value);
        }

        #endregion

        /// <summary>
        /// 指定されたIDのプレイリストを含むかどうかを返す
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool Contains(int id)
        {
            return this.innertPlaylists.ContainsKey(id);
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
        /// プレイリストを空にして追加する
        /// </summary>
        /// <param name="playlists"></param>
        public void Initialize(List<ITreePlaylistInfo> playlists)
        {
            this.MergeRange(playlists, true);

            var tree = this.GetTree();
            this.Playlists.Clear();
            this.Playlists.Add(tree);
        }

        #region private
        /// <summary>
        /// ルートプレイリストを取得する
        /// </summary>
        /// <returns></returns>
        private ITreePlaylistInfo? GetRoot()
        {
            return this.GetAllPlaylists().FirstOrDefault(p => p.IsRoot);
        }


        /// <summary>
        /// 完全なプレイリストツリーを構築する
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private ITreePlaylistInfo ConstructPlaylistInfo(int id)
        {
            foreach (var self in this.innertPlaylists)
            {
                var parentID = self.Value.ParentId;
                if (parentID >= 0)
                {
                    var parent = this.GetPlaylist(parentID);
                    if (parent is not null)
                    {
                        parent.Children.Add(self.Value);
                    }
                }
            }

            var playlist = this.GetPlaylist(id)!;

            return playlist;
        }
        #endregion
    }
}
