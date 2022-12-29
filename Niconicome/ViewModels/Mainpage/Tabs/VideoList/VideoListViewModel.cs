using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Const = Niconicome.Models.Const;
using WS = Niconicome.Workspaces;

namespace Niconicome.ViewModels.Mainpage.Tabs.VideoList
{
    public class VideoListViewModel : TabViewModelBase
    {
        public VideoListViewModel() : base("動画一覧", Const::LocalConstant.VideoListV2TabID)
        {

        }

        #region field

        private Action? _listChangedEventHandler;

        #endregion

        #region Props

        public List<VideoInfoViewModel> Videos { get; init; } = new List<VideoInfoViewModel>();

        #endregion

        #region Method

        /// <summary>
        /// 動画リスト変更イベントハンドラを追加
        /// </summary>
        /// <param name="handler"></param>
        public void RegisterListChangedEventHandler(Action handler)
        {
            this._listChangedEventHandler += handler;
        }

        /// <summary>
        /// 初期化
        /// </summary>
        public void Initialize()
        {
            this.Videos.Clear();
            this.Videos.AddRange(WS::Mainpage.PlaylistVideoContainer.Videos.Select(v => new VideoInfoViewModel(v)));
        }

        #endregion
    }
}
