using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Microsoft.Xaml.Behaviors;
using Niconicome.Models.Playlist.Playlist;
using WS = Niconicome.Workspaces;

namespace Niconicome.ViewModels.Mainpage.PlaylistTree
{
    public class PlaylistTreeBehavior : Behavior<TreeView>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.SelectedItemChanged += this.OnSelectedChanged;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            this.AssociatedObject.SelectedItemChanged -= this.OnSelectedChanged;
        }

        /// <summary>
        /// プレイリストの並び替えタイプ
        /// </summary>
        private InsertType insertType = InsertType.None;

        /// <summary>
        /// 選択したアイテムが変更されたとき
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSelectedChanged(object? sender, EventArgs e)
        {
            //nullの場合はキャンセル
            if (this.AssociatedObject.SelectedItem is null) return;

            //キャストできない場合はキャンセル
            if (this.AssociatedObject.SelectedItem  is not PlaylistInfoViewModel vm) return;

            //nullの場合はキャンセル
            if (vm is null) return;

            //変更がない場合はキャンセル
            if (WS::Mainpage.PlaylistVideoContainer.CurrentSelectedPlaylist is not null && WS::Mainpage.PlaylistVideoContainer.CurrentSelectedPlaylist.ID == vm.ID) return;

            //ルートプレイリストは選択禁止
            if (vm.IsRoot) return;

            //末端プレイリストでない場合はキャンセル
            if (vm.Children.Count > 0) return;

            WS::Mainpage.PlaylistVideoContainer.CurrentSelectedPlaylist = vm.PlaylistInfo;
        }
    }
}
