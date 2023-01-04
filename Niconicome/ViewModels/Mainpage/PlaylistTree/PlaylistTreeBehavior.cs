using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Microsoft.Xaml.Behaviors;
using Niconicome.Models.Playlist.Playlist;

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
            if (this.AssociatedObject.SelectedItem  is not PlaylistInfoViewModel playlistInfo) return;

            //nullの場合はキャンセル
            if (playlistInfo is null) return;

            //変更がない場合はキャンセル
            //if (WS::Mainpage.CurrentPlaylist.SelectedPlaylist.Value?.Id == playlistInfo.Id) return;

            //ルートプレイリストは選択禁止
            if (playlistInfo.IsRoot) return;

            //末端プレイリストでない場合はキャンセル
            if (playlistInfo.Children.Count > 0) return;

            //WS::Mainpage.CurrentPlaylist.SelectedPlaylist.Value = playlistInfo;
        }
    }
}
