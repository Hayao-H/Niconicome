using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Utils.Reactive;
using WS = Niconicome.Workspaces.Mainpage;
using VideoInfo = Niconicome.Models.Domain.Niconico.Video.Infomations;

namespace Niconicome.ViewModels.Mainpage.Tabs.VideoList.BottomPanel
{
    public class DownloadSettingsViewModel : IDisposable
    {
        public DownloadSettingsViewModel()
        {
            this.DownloadVideo = WS.DownloadSettingsHandler.IsDownloadingVideoEnable.Bindablevalue.AddTo(this.Bindables);
            this.DownloadComment = WS.DownloadSettingsHandler.IsDownloadingCommentEnable.Bindablevalue.AddTo(this.Bindables);
            this.DownloadOwnerComment = WS.DownloadSettingsHandler.IsDownloadingOwnerComment.Bindablevalue.AddTo(this.Bindables);
            this.DownloadEasyComment = WS.DownloadSettingsHandler.IsDownloadingEasyComment.Bindablevalue.AddTo(this.Bindables);
            this.DownloadThumbnail = WS.DownloadSettingsHandler.IsDownloadingThumbEnable.Bindablevalue.AddTo(this.Bindables);
            this.DownloadVideoInfo = WS.DownloadSettingsHandler.IsDownloadingVideoInfoEnable.Bindablevalue.AddTo(this.Bindables);
            this.DownloadCommentLog = WS.DownloadSettingsHandler.IsDownloadingCommentLogEnable.Bindablevalue.AddTo(this.Bindables);
            this.IsCommentLimitEnable = WS.DownloadSettingsHandler.IsLimittingCommentCountEnable.Bindablevalue.AddTo(this.Bindables);
            this.CommentLimit = WS.DownloadSettingsHandler.MaxCommentsCount.Bindablevalue.AddTo(this.Bindables);
            this.Resolution = new BindableProperty<string>(WS.DownloadSettingsHandler.Resolution.Value.Vertical.ToString()).Subscribe(x =>
            {
                WS.DownloadSettingsHandler.Resolution.Value = x switch
                {
                    "1080" => new VideoInfo::Resolution("1920x1080"),
                    "720" => new VideoInfo::Resolution("1280x720"),
                    "480" => new VideoInfo::Resolution("854x480"),
                    "360" => new VideoInfo::Resolution("640x360"),
                    _ => new VideoInfo::Resolution("426x240"),
                };
            }).AddTo(this.Bindables);
            this.ThumbSize =new BindableProperty<string>(WS.DownloadSettingsHandler.ThumbnailSize.Value switch {
                VideoInfo::ThumbSize.Large => "大",
                VideoInfo::ThumbSize.Middle => "中",
                VideoInfo::ThumbSize.Normal => "普通",
                _=>"プレイヤー"
            }).Subscribe(x =>
            {
                WS.DownloadSettingsHandler.ThumbnailSize.Value = x switch
                {
                    "大" => VideoInfo::ThumbSize.Large,
                    "中" => VideoInfo::ThumbSize.Middle,
                    "普通" => VideoInfo::ThumbSize.Normal,
                    _ => VideoInfo::ThumbSize.Player
                };
            }).AddTo(this.Bindables);
            this.OverwriteEnable = WS.DownloadSettingsHandler.IsOverwriteEnable.Bindablevalue.AddTo(this.Bindables);
            this.SkipDownloaded = WS.DownloadSettingsHandler.IsSkippingEnable.Bindablevalue.AddTo(this.Bindables);
            this.AppendComment = WS.DownloadSettingsHandler.IsAppendingCommentEnable.Bindablevalue.AddTo(this.Bindables);
            this.IsDownloading = WS.DownloadManager.IsProcessing.AddTo(this.Bindables);

        }

        public Bindables Bindables { get; init; } = new();

        /// <summary>
        /// 動画をダウンロードするかどうか
        /// </summary>
        public IBindableProperty<bool> DownloadVideo { get; init; }

        /// <summary>
        /// コメントをダウンロードするかどうか
        /// </summary>
        public IBindableProperty<bool> DownloadComment { get; init; }

        /// <summary>
        /// 投稿者コメントをダウンロードするかどうか
        /// </summary>
        public IBindableProperty<bool> DownloadOwnerComment { get; init; }

        /// <summary>
        /// 過去ログをダウンロードするかどうか
        /// </summary>
        public IBindableProperty<bool> DownloadCommentLog { get; init; }

        /// <summary>
        /// かんたんコメントをダウンロードするかどうか
        /// </summary>
        public IBindableProperty<bool> DownloadEasyComment { get; init; }

        /// <summary>
        /// サムネイルをダウンロードするかどうか
        /// </summary>
        public IBindableProperty<bool> DownloadThumbnail { get; init; }

        /// <summary>
        /// 動画情報をダウンロードするかどうか
        /// </summary>
        public IBindableProperty<bool> DownloadVideoInfo { get; init; }

        /// <summary>
        /// コメント数を制限するかどうか
        /// </summary>
        public IBindableProperty<bool> IsCommentLimitEnable { get; init; }

        /// <summary>
        /// 最大コメント数
        /// </summary>
        public IBindableProperty<int> CommentLimit { get; init; }

        /// <summary>
        /// 解像度
        /// </summary>
        public IBindableProperty<string> Resolution { get; init; }

        /// <summary>
        /// サムネイルのサイズ
        /// </summary>
        public IBindableProperty<string> ThumbSize { get; init; }

        /// <summary>
        /// 道明寺に上書き
        /// </summary>
        public IBindableProperty<bool> OverwriteEnable { get; init; }

        /// <summary>
        /// ダウンロード済みの動画をスキップするかどうか
        /// </summary>
        public IBindableProperty<bool> SkipDownloaded { get; init;}

        /// <summary>
        /// コメントを追記するかどうか
        /// </summary>
        public IBindableProperty<bool> AppendComment { get; init; }

        /// <summary>
        /// ダウンロードフラグ
        /// </summary>
        public IReadonlyBindablePperty<bool> IsDownloading { get; init; }

        /// <summary>
        /// 動画をステージ
        /// </summary>
        public void StageVideos()
        {
            if (WS.PlaylistVideoContainer.CurrentSelectedPlaylist is null)
            {
                WS.SnackbarHandler.Enqueue("プレイリストが選択されていないため、ステージできません");
                return;
            }

            if (!this.DownloadComment.Value && (this.DownloadOwnerComment.Value || this.DownloadEasyComment.Value || this.DownloadCommentLog.Value))
            {
                WS.SnackbarHandler.Enqueue("過去ログ・投コメ・かんたんコメントをDLするにはコメントにチェックを入れてください。");
                return;
            }

            if (!this.DownloadVideo.Value && !this.DownloadComment.Value && !this.DownloadThumbnail.Value && !this.DownloadVideoInfo.Value) return;

            foreach (var video in WS.PlaylistVideoContainer.Videos.Where(v => v.IsSelected.Value))
            {
                WS.DownloadManager.StageVIdeo(video);
            }

            WS.SnackbarHandler.Enqueue($"選択された動画をステージしました。", "管理画面を開く", () =>
            {
                WS.TabControler.Open(Models.Local.State.Tab.V1.TabType.Download);
            });
        }

        /// <summary>
        /// ダウンロードを開始
        /// </summary>
        public async Task Download()
        {
            if (WS.PlaylistVideoContainer.CurrentSelectedPlaylist is null)
            {
                WS.SnackbarHandler.Enqueue("プレイリストが選択されていないため、ダウンロードできません");
                return;
            }

            if (!WS.Session.IsLogin.Value)
            {
                WS.SnackbarHandler.Enqueue("動画をダウンロードするにはログインが必要です。");
                return;
            }

            if (!this.DownloadComment.Value && (this.DownloadOwnerComment.Value || this.DownloadEasyComment.Value || this.DownloadCommentLog.Value))
            {
                WS.SnackbarHandler.Enqueue("過去ログ・投コメ・かんたんコメントをDLするにはコメントにチェックを入れてください。");
                return;
            }

            if (!this.DownloadVideo.Value && !this.DownloadComment.Value && !this.DownloadThumbnail.Value && !this.DownloadVideoInfo.Value) return;


            foreach (var video in WS.PlaylistVideoContainer.Videos.Where(v => v.IsSelected.Value))
            {
                WS.DownloadManager.StageVIdeo(video);
            }

            await WS.DownloadManager.StartDownloadAsync(m => WS.SnackbarHandler.Enqueue(m), m => WS.Messagehandler.AppendMessage(m));
            WS.PostDownloadTasksManager.HandleAction();
        }

        public void Cancel()
        {
            WS.DownloadManager.CancelDownload();
        }


        public void Dispose()
        {
            this.Bindables.Dispose();
        }

    }


}
