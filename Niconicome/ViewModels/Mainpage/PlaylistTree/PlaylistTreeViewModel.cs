﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Const;
using Niconicome.Models.Domain.Playlist;
using Niconicome.Models.Domain.Utils.Error;
using Niconicome.Models.Helper.Result;
using Niconicome.Models.Local.State;
using Niconicome.Models.Utils.Reactive;
using Niconicome.Models.Utils.Reactive.Command;
using Niconicome.ViewModels.Controls;
using WS = Niconicome.Workspaces;

namespace Niconicome.ViewModels.Mainpage.PlaylistTree
{
    public class PlaylistTreeViewModel
    {
        public PlaylistTreeViewModel()
        {
            this.Playlists = new BindableCollection<PlaylistInfoViewModel, IPlaylistInfo>(WS::Mainpage.PlaylistVideoContainer.Playlist, info => new PlaylistInfoViewModel(info)).AsReadOnly();

            this.AddPlaylist = new BindableCommand<int>(parentID =>
            {
                IAttemptResult result = WS::Mainpage.PlaylistManager.Create(parentID);
                if (result.IsSucceeded)
                {
                    WS::Mainpage.MessageHandler.AppendMessage(WS::Mainpage.StringHandler.GetContent(PlaylistTreeViewModelStringContent.PlaylistCreated), LocalConstant.SystemMessageDispacher, ErrorLevel.Log);
                    WS::Mainpage.SnackbarHandler.Enqueue(WS::Mainpage.StringHandler.GetContent(PlaylistTreeViewModelStringContent.PlaylistCreated));
                }
                else
                {
                    WS::Mainpage.MessageHandler.AppendMessage(result.Message ?? "", LocalConstant.SystemMessageDispacher, result.ErrorLevel);
                }

            });

            this.RemovePlaylist = new BindableCommand<int>(async id =>
            {
                //削除前に確認
                MaterialMessageBoxResult confirmResult = await MaterialMessageBox.Show(WS::Mainpage.StringHandler.GetContent(PlaylistTreeViewModelStringContent.ConfirmOfDeletion), MessageBoxButtons.Yes | MessageBoxButtons.Cancel, MessageBoxIcons.Question);
                if (confirmResult != MaterialMessageBoxResult.Yes) return;

                IAttemptResult result = WS::Mainpage.PlaylistManager.Delete(id);
                if (result.IsSucceeded)
                {
                    WS::Mainpage.MessageHandler.AppendMessage(WS::Mainpage.StringHandler.GetContent(PlaylistTreeViewModelStringContent.PlaylistDeleted, id), LocalConstant.SystemMessageDispacher, ErrorLevel.Log);
                    WS::Mainpage.SnackbarHandler.Enqueue(WS::Mainpage.StringHandler.GetContent(PlaylistTreeViewModelStringContent.PlaylistDeleted, id));
                }
                else
                {
                    WS::Mainpage.MessageHandler.AppendMessage(result.Message ?? "", LocalConstant.SystemMessageDispacher, result.ErrorLevel);
                }
            });

            this.EditPlaylist = new BindableCommand<int>(id =>
            {
                WS::Mainpage.BlazorPageManager.RequestBlazorToNavigate($"/playlist/{id}");
            });

            this.Refresh = new BindableCommand(() =>
            {
                WS::Mainpage.PlaylistManager.Initialize();
            });

            WS::Mainpage.PlaylistManager.Initialize();
        }

        #region Props

        public ReadOnlyObservableCollection<PlaylistInfoViewModel> Playlists { get; init; }

        public IBindableCommand<int> AddPlaylist { get; init; }

        public IBindableCommand<int> RemovePlaylist { get; init; }

        public IBindableCommand<int> EditPlaylist { get; init; }

        public IBindableCommand Refresh { get; init; }

        #endregion

        #region Method



        #endregion

    }
}
