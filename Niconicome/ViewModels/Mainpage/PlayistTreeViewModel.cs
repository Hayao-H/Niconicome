using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Xaml.Behaviors;
using Niconicome.Extensions;
using Niconicome.Extensions.System.Windows;
using Niconicome.Models.Playlist;
using Niconicome.ViewModels.Controls;
using Niconicome.ViewModels.Mainpage.Subwindows;
using Niconicome.Views;
using Utils = Niconicome.Models.Domain.Utils;
using WS = Niconicome.Workspaces;

namespace Niconicome.ViewModels.Mainpage
{

    /// <summary>
    /// プレイリストツリーのVM
    /// </summary>
    class PlaylistTreeViewModel : BindableBase
    {
        public PlaylistTreeViewModel() : this((text, button, icon) => MaterialMessageBox.Show(text, button, icon)) { }

        public PlaylistTreeViewModel(Func<string, MessageBoxButtons, MessageBoxIcons, Task<MaterialMessageBoxResult>> showMessageBox)
        {
            this.ShowMessageBox = showMessageBox;

            //初期化失敗時
            void initializeFailed()
            {
                this.ShowMessageBox("アプリケーションの初期化中にエラーが発生しました。詳しくはエラーログをご確認下さい。", MessageBoxButtons.OK, MessageBoxIcons.Error);
                Application.Current?.Shutdown();
            }

            try
            {
                this.PlaylistTreeHandler = WS::Mainpage.PlaylistTree;
            }
            catch (Exception e)
            {
                Utils::Logger.GetLogger().Error("初期化中にエラーが発生しました。", e);
                initializeFailed();
            }

#pragma warning disable CS8622
            this.AddPlaylist = new CommandBase<int>((object? arg) => true, (int parent) =>
            {
                WS::Mainpage.PlaylistTree.AddPlaylist(parent);
                this.OnPropertyChanged(nameof(this.PlaylistTree));
            });
            this.RemovePlaylist = new CommandBase<int>((object? arg) => true, (int playlist) =>
            {
                WS::Mainpage.PlaylistTree.DeletePlaylist(playlist);
                this.OnPropertyChanged(nameof(this.PlaylistTree));
            });
#pragma warning restore CS8622

            this.PlaylistRefreshcommand = new CommandBase<object>(_ => true, _ =>
            {
                WS::Mainpage.PlaylistTree.Refresh();
                WS::Mainpage.CurrentPlaylist.Update(WS::Mainpage.CurrentPlaylist.CurrentSelectedPlaylist?.Id ?? -1);
                this.OnPropertyChanged(nameof(this.PlaylistTree));
            });

            this.EditPlaylistCommand = new CommandBase<object>(_ => true, arg =>
            {
                if (arg is null || arg.AsNullable<ITreePlaylistInfo>() is not ITreePlaylistInfo playlist || playlist is null) return;
                var window = new EditPlaylist
                {
                    Owner = Application.Current.MainWindow,
                    DataContext = new PlaylistEditViewModel(playlist)
                };
                window.Show();
            });
        }

        public void OnPropertyChangedPublic(string prop)
        {
            this.OnPropertyChanged(prop);
        }

        /// <summary>
        /// メッセージボックスを表示する
        /// </summary>
        private Func<string, MessageBoxButtons, MessageBoxIcons, Task<MaterialMessageBoxResult>> ShowMessageBox { get; set; }

        /// <summary>
        /// プレイリストのツリー
        /// </summary>
        public IPlaylistVideoHandler? PlaylistTreeHandler { get; private set; }

        /// <summary>
        /// プレイリストのツリー
        /// </summary>
        public ObservableCollection<ITreePlaylistInfo> PlaylistTree
        {
            get
            {
                return this.PlaylistTreeHandler?.Playlists ?? new ObservableCollection<ITreePlaylistInfo>();
            }
        }

        /// <summary>
        /// プレイリスト追加
        /// </summary>
        public CommandBase<int> AddPlaylist { get; private set; }

        /// <summary>
        /// プレイリスト削除
        /// </summary>
        public CommandBase<int> RemovePlaylist { get; private set; }

        /// <summary>
        /// プレイリストを更新する
        /// </summary>
        public CommandBase<object> PlaylistRefreshcommand { get; init; }

        /// <summary>
        /// プレイリストを編集
        /// </summary>
        public CommandBase<object> EditPlaylistCommand { get; init; }
    }

    /// <summary>
    /// プレイリストツリーのビヘビアー
    /// </summary>
    class PlaylistTreeBehavior : Behavior<TreeView>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.SelectedItemChanged += this.OnSelectedChanged;
            this.AssociatedObject.AllowDrop = true;
            this.AssociatedObject.PreviewMouseMove += this.OnMouseMove;
            this.AssociatedObject.PreviewMouseLeftButtonDown += this.OnLeftButtonDown;
            this.AssociatedObject.DragOver += this.OnDragOver;
            this.AssociatedObject.Drop += this.OnDrop;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            this.AssociatedObject.SelectedItemChanged -= this.OnSelectedChanged;
            this.AssociatedObject.PreviewMouseMove -= this.OnMouseMove;
            this.AssociatedObject.PreviewMouseLeftButtonDown -= this.OnLeftButtonDown;
            this.AssociatedObject.DragOver -= this.OnDragOver;
            this.AssociatedObject.Drop -= this.OnDrop;
        }

        /// <summary>
        /// ドラッグを開始した地点(左クリック地点)
        /// </summary>
        private Point? startPoint;

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
            if (this.AssociatedObject.SelectedItem.As<ITreePlaylistInfo>() is not ITreePlaylistInfo playlistInfo) return;

            //変更がない場合はキャンセル
            if (WS::Mainpage.CurrentPlaylist.CurrentSelectedPlaylist?.Id == playlistInfo.Id) return;

            //ルートプレイリストは選択禁止
            if (playlistInfo.IsRoot) return;

            //末端プレイリストでない場合はキャンセル
            if (playlistInfo.ChildrensIds.Count > 0) return;

            WS::Mainpage.CurrentPlaylist.CurrentSelectedPlaylist = playlistInfo;
        }

        /// <summary>
        /// 左ボタン押下時
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnLeftButtonDown(object? sender, MouseEventArgs e)
        {
            //クリックしたアイテムの妥当性を検証
            if (this.AssociatedObject.SelectedItem is null || sender is not ItemsControl itemsControl) return;
            var elm = this.HitTest<FrameworkElement>(itemsControl, e.GetPosition(itemsControl));
            //クリックした要素がプレイリスト出ない場合キャンセル
            if (elm == null || elm.DataContext == null || elm.DataContext is not NonBindableTreePlaylistInfo) return;
            //クリックしたプレイリストがルートの場合キャンセル
            NonBindableTreePlaylistInfo? clickedList = elm.DataContext.AsNullable<NonBindableTreePlaylistInfo>();
            if (clickedList?.IsRoot ?? false) return;

            this.startPoint = e.GetPosition(itemsControl);
        }

        /// <summary>
        /// ドラッグを開始
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            //ドラッグ動作が可能な状態であるかどうかを検証
            if (this.AssociatedObject.SelectedItem is null || this.startPoint is null || sender is not TreeView) return;

            //ドラッグ可能であるかどうかを判断する
            Point mousePoint = this.AssociatedObject.PointToScreen(e.GetPosition(this.AssociatedObject));
            Vector diff = (Point)this.startPoint - mousePoint;

            if (!this.CanDrag(diff)) return;

            DragDrop.DoDragDrop(this.AssociatedObject, this.AssociatedObject.SelectedItem, DragDropEffects.Move);
            this.startPoint = null;
        }

        /// <summary>
        /// ドラッグ時
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDragOver(object? sender, DragEventArgs e)
        {
            this.ResetState();

            //ドラッグの妥当性を検証
            if (sender is null || sender is not ItemsControl || !e.Data.GetDataPresent(typeof(BindableTreePlaylistInfo))) return;

            var itemsControl = sender.As<ItemsControl>();

            //データコンテキストを取得
            PlaylistTreeViewModel? context = itemsControl.DataContext.AsNullable<PlaylistTreeViewModel>();
            if (context is null) return;

            //ドラッグ中のアイテム
            ITreePlaylistInfo sourceInfo = e.Data.GetData(typeof(BindableTreePlaylistInfo)).As<ITreePlaylistInfo>();
            var targetElement = this.HitTest<FrameworkElement>(itemsControl, e.GetPosition(itemsControl));

            //ドラッグ先を発見できなかった場合キャンセル
            if (targetElement is null) return;

            ////ドロップ先の要素を取得
            var parentItem = targetElement.GetParent<TreeViewItem>();

            ////ドロップ先の情報を取得
            if (parentItem is null || parentItem.DataContext.AsNullable<ITreePlaylistInfo>() is not ITreePlaylistInfo targetInfo) return;

            //ルートは禁止
            if (sourceInfo.IsRoot) return;

            //自分自身には挿入できない
            if (sourceInfo.Id == targetInfo.Id) return;

            ///ToDo:並び替え機能
            ///→実装したくなったら下のコメントアウトを解除してif文を整える
            ///→DataBase側の実装が無いので、Sequenceプロパティーを持たせる
            ///
            ///Point pos = e.GetPosition(parentItem);
            ///var judgement = 10;
            ///  
            ///if (pos.Y > 0 && pos.Y < judgement)
            ///{
            ///    this.insertType = InsertType.Before;
            ///    targetInfo.BeforeSeparatorVisibility = Visibility.Visible;
            ///}
            ///else if (context.PlaylistTreeHandler?.IsLastChild(targetInfo.Id) ?? false && pos.Y > targetElement.ActualHeight - judgement && pos.Y < targetElement.ActualHeight)
            ///{
            ///    this.insertType = InsertType.After;
            ///    targetInfo.AfterSeparatorVisibility = Visibility.Visible;
            ///}
            //else if (!targetInfo.IsConcrete&&targetInfo.Id!=sourceInfo.ParentId)

            //自分の親に入れても状況は変わらないのでキャンセル
            //また、動画を保持しているプレイリストは親にはなれないのでキャンセル
            if (!targetInfo.IsConcrete && targetInfo.Id != sourceInfo.ParentId)
            {
                this.insertType = InsertType.Child;
                Color color = (Color)ColorConverter.ConvertFromString("#e0e0e0");
                targetInfo.BackgroundColor = new SolidColorBrush(color);
                parentItem.Background = new SolidColorBrush(color);
            }
            else
            {
                this.insertType = InsertType.None;
            }

            this.stateChangedPlaylists.Add(targetInfo);

        }

        /// <summary>
        /// ドロップ時
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDrop(object? sender, DragEventArgs e)
        {

            this.ResetState();

            if (this.insertType == InsertType.None) return;

            //ドロップイベントの合理性を検証
            if (sender is null || sender is not ItemsControl itemsControl || !e.Data.GetDataPresent(typeof(BindableTreePlaylistInfo))) return;

            //ドロップしたアイテムを取得
            ITreePlaylistInfo? sourceInfo = e.Data.GetData(typeof(BindableTreePlaylistInfo)).AsNullable<ITreePlaylistInfo>();

            //nullチェック
            if (sourceInfo is null) return;

            //ドロップ先を取得
            var targetElement = this.HitTest<FrameworkElement>(itemsControl, e.GetPosition(itemsControl));
            if (targetElement is null) return;

            //親アイテムを取得
            var parentElement = targetElement.GetParent<TreeViewItem>();
            if (parentElement is null || parentElement?.DataContext?.AsNullable<ITreePlaylistInfo>() is not ITreePlaylistInfo parentInfo) return;

            //データコンテキストを取得
            PlaylistTreeViewModel? context = this.AssociatedObject.DataContext.AsNullable<PlaylistTreeViewModel>();
            if (context is null) return;

            if (this.insertType == InsertType.Child)
            {
                context.PlaylistTreeHandler?.Move(sourceInfo.Id, parentInfo.Id);
                this.insertType = InsertType.None;
            }
        }

        /// <summary>
        /// ドラック可能であるかどうかを判断する
        /// </summary>
        /// <param name="diff"></param>
        /// <returns></returns>
        private bool CanDrag(Vector diff)
        {
            return (SystemParameters.MinimumHorizontalDragDistance < Math.Abs(diff.X) && SystemParameters.MinimumVerticalDragDistance < Math.Abs(diff.Y));
        }

        /// <summary>
        /// 何らかの変更が行われたプレイリスト
        /// </summary>
        private readonly List<ITreePlaylistInfo> stateChangedPlaylists = new();

        /// <summary>
        /// 状態の変更を元に戻す
        /// </summary>
        private void ResetState()
        {
            //変更が内場合はキャンセル
            if (this.stateChangedPlaylists.Count == 0) return;

            foreach (var playlist in this.stateChangedPlaylists)
            {
                //前後のバーを非表示に戻す
                playlist.AfterSeparatorVisibility = Visibility.Hidden;
                playlist.BeforeSeparatorVisibility = Visibility.Hidden;
                playlist.BackgroundColor = Brushes.Transparent;
            }

            //状態が変更されたプレイリストは存在しないので削除
            this.stateChangedPlaylists.Clear();
        }

        /// <summary>
        /// カーソルの下にある要素を取得する
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="itemsControl"></param>
        /// <param name="getPosition"></param>
        /// <returns></returns>
        private T? HitTest<T>(UIElement itemsControl, Point pos) where T : class
        {
            var result = itemsControl.InputHitTest(pos).As<DependencyObject>();
            return result switch
            {
                T elm => elm,
                null => null,
                _ => null
            };
        }

    }
}
