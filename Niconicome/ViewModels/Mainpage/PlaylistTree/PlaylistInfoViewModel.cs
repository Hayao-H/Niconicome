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
            this._info = info;
        }

        #region field

        private readonly IPlaylistInfo _info;

        #endregion

        #region Props

        /// <summary>
        /// ID
        /// </summary>
        public int ID => this._info.ID;

        /// <summary>
        /// プレイリスト名
        /// </summary>
        public BindableProperty<string> Name => this._info.Name;

        /// <summary>
        /// 動画数
        /// </summary>
        public int VideosCount =>this._info.Videos.Count;

        /// <summary>
        /// ルートプレイリストであるかどうか
        /// </summary>
        public bool IsRoot => this._info.PlaylistType == PlaylistType.Root;

        /// <summary>
        /// 子プレイリスト(デフォルトでは空)
        /// </summary>
        public ReadOnlyObservableCollection<IPlaylistInfo> Children => this._info.Children;

        #endregion

    }
}
