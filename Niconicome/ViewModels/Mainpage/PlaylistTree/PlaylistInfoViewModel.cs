using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Playlist;
using Niconicome.Models.Utils.Reactive;
using Reactive.Bindings;

namespace Niconicome.ViewModels.Mainpage.PlaylistTree
{
    public class PlaylistInfoViewModel
    {
        public PlaylistInfoViewModel(IPlaylistInfo info)
        {
            this.PlaylistInfo = info;
            this.Children = new BindableCollection<PlaylistInfoViewModel, IPlaylistInfo>(info.Children, info => new PlaylistInfoViewModel(info)).AsReadOnly();
        }

        #region Props

        /// <summary>
        /// IPlaylistInfo
        /// </summary>
        public IPlaylistInfo PlaylistInfo { get; init; }

        /// <summary>
        /// ID
        /// </summary>
        public int ID => this.PlaylistInfo.ID;

        /// <summary>
        /// プレイリスト名
        /// </summary>
        public BindableProperty<string> Name => this.PlaylistInfo.Name;

        /// <summary>
        /// 動画数
        /// </summary>
        public int VideosCount => this.PlaylistInfo.Videos.Count;

        /// <summary>
        /// ルートプレイリストであるかどうか
        /// </summary>
        public bool IsRoot => this.PlaylistInfo.PlaylistType == PlaylistType.Root;

        /// <summary>
        /// 展開状況
        /// </summary>
        public IBindableProperty<bool> IsExpanded => this.PlaylistInfo.IsExpanded;

        /// <summary>
        /// 子プレイリスト(デフォルトでは空)
        /// </summary>
        public ReadOnlyObservableCollection<PlaylistInfoViewModel> Children { get; init; }

        #endregion

    }
}
