using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AngleSharp.Dom;
using Niconicome.Extensions.System.List;
using Niconicome.Models.Local.Settings;
using Reactive.Bindings.ObjectExtensions;
using Windows.ApplicationModel.Payments;

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
        ITreePlaylistInfo GetTmp();
        IEnumerable<ITreePlaylistInfo> GetAllPlaylists();
        List<string> GetListOfAncestor(int id);
        ObservableCollection<ITreePlaylistInfo> Playlists { get; }
    }

    /// <summary>
    /// TreePlaylistInfoをよしなにしてくれる
    /// (実際にはインスタンスを管理している)
    /// </summary>
    public class PlaylistTreeHandler : IPlaylistTreeHandler
    {
        public PlaylistTreeHandler(IPlaylistSettingsHandler playlistSettingsHandler)
        {
            this.playlistSettingsHandler = playlistSettingsHandler;
        }

        /// <summary>
        /// 内部プレイリスト(辞書で管理)
        /// </summary>
        private readonly Dictionary<int, ITreePlaylistInfo> innertPlaylists = new();

        /// <summary>
        /// プレイリスト
        /// </summary>
        public ObservableCollection<ITreePlaylistInfo> Playlists { get; init; } = new();

        #region field

        private readonly IPlaylistSettingsHandler playlistSettingsHandler;

        #endregion

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

                //重複があった場合は更新する
                ITreePlaylistInfo? before = this.GetPlaylist(after.Id);
                if (before is not null)
                {
                    after.BeforeSeparatorVisibility = before.BeforeSeparatorVisibility;
                    after.AfterSeparatorVisibility = before.AfterSeparatorVisibility;
                    after.IsExpanded = before.IsExpanded;
                    //before.UpdateData(after);
                    continue;
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
        /// 一時プレイリストを取得
        /// </summary>
        /// <returns></returns>
        public ITreePlaylistInfo GetTmp()
        {
            return this.Playlists.First(p => p.IsTemporary);
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
        /// 親プレイリストのリストを取得する
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<string> GetListOfAncestor(int id)
        {
            ITreePlaylistInfo? playlist = this.GetPlaylist(id);

            if (playlist is null)
            {
                return new List<string>();
            }

            var ancester = new LinkedList<string>();
            ancester.AddFirst(playlist.Name.Value);

            ITreePlaylistInfo? parent = this.GetParent(playlist.Id);

            while (parent is not null)
            {
                ancester.AddFirst(parent.Name.Value);
                parent = this.GetParent(parent.Id);
            }

            return ancester.ToList();
        }

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

            if (parent.Children.Count == 0) return false;

            return parent.Children.Last().Id == id;
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
            this.Playlists.Addrange(tree);
        }

        #region private

        /// <summary>
        /// ツリーを取得する
        /// </summary>
        /// <returns></returns>
        private List<ITreePlaylistInfo> GetTree()
        {
            var list = new List<ITreePlaylistInfo>();

            ITreePlaylistInfo? root = this.GetRoot();

            //ルートプレイリストがnullの場合はエラーを返す
            if (root == null) throw new InvalidOperationException("ルートプレイリストが存在しません。");

            ITreePlaylistInfo? tmp = this.GetPlaylist(p => p.IsTemporary);
            ITreePlaylistInfo? succeeded = this.GetPlaylist(p => p.IsDownloadSucceededHistory);
            ITreePlaylistInfo? failed = this.GetPlaylist(p => p.IsDownloadFailedHistory);

            bool succeededDisable = this.playlistSettingsHandler.IsDownloadSucceededHistoryDisabled;
            bool failedDisable = this.playlistSettingsHandler.IsDownloadFailedHistoryDisabled;

            if (tmp is null)
            {
                throw new InvalidOperationException("一時プレイリストが存在しません。");
            }
            else if (!succeededDisable && succeeded is null)
            {
                throw new InvalidOperationException("DL成功履歴プレイリストが存在しません。");
            }
            else if (!failedDisable && failed is null)
            {
                throw new InvalidOperationException("DL失敗履歴プレイリストが存在しません。");
            }

            list.Add(this.ConstructPlaylistInfo(root.Id));
            list.Add(tmp);

            if (!failedDisable)
            {
                list.Add(failed!);
            }
            if (!succeededDisable)
            {
                list.Add(succeeded!);
            }

            return list;

        }

        /// <summary>
        /// ルートプレイリストを取得する
        /// </summary>
        /// <returns></returns>
        private ITreePlaylistInfo? GetRoot()
        {
            return this.GetAllPlaylists().FirstOrDefault(p => p.IsRoot);
        }

        /// <summary>
        /// 条件を指定してプレイリストを取得する
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        private ITreePlaylistInfo? GetPlaylist(Func<ITreePlaylistInfo, bool> predicate)
        {
            return this.innertPlaylists.FirstOrDefault(kv => predicate(kv.Value)).Value;
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
                        if (!parent.Children.Any(p => p.Id == self.Value.Id))
                        {
                            parent.Children.Add(self.Value);
                        }
                    }
                }
            }

            var playlist = this.GetPlaylist(id)!;

            return playlist;
        }
        #endregion
    }
}
