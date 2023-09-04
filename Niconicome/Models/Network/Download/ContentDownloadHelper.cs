using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Niconicome.Models.Domain.Niconico.Download;
using Niconicome.Models.Domain.Niconico.Download.Ichiba;
using Niconicome.Models.Domain.Utils;
using Niconicome.Models.Helper.Result;
using Niconicome.Models.Local.Settings;
using Niconicome.Models.Network.Watch;
using Niconicome.Models.Playlist;
using Cdl = Niconicome.Models.Domain.Niconico.Download.Comment;
using DDL = Niconicome.Models.Domain.Niconico.Download.Description;
using Tdl = Niconicome.Models.Domain.Niconico.Download.Thumbnail;
using Vdl = Niconicome.Models.Domain.Niconico.Download.Video.V2;
using V2Comment = Niconicome.Models.Domain.Niconico.Download.Comment.V2.Integrate;
using Niconicome.Models.Domain.Playlist;
using Niconicome.Models.Domain.Niconico.Download.Video;
using Niconicome.Models.Domain.Niconico.Watch;
using Niconicome.Models.Domain.Niconico.Video.Infomations;
using Niconicome.Models.Domain.Niconico.Watch.V2;

namespace Niconicome.Models.Network.Download
{
    public interface IContentDownloadHelper
    {
        /// <summary>
        /// 非同期でコンテンツをダウンロードする
        /// </summary>
        /// <param name="videoInfo"></param>
        /// <param name="setting"></param>
        /// <param name="OnMessage"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<IAttemptResult<IDownloadContext>> TryDownloadContentAsync(IVideoInfo videoInfo, IDownloadSettings setting, Action<string> OnMessage, CancellationToken token);
    }

    public class ContentDownloadHelper : IContentDownloadHelper
    {
        public ContentDownloadHelper(ILogger logger, ILocalContentHandler localContentHandler, IDomainModelConverter converter, IWatchPageInfomationHandler watchPageInfomation)
        {
            this.localContentHandler = localContentHandler;
            this.converter = converter;
            this.logger = logger;
            this._watch = watchPageInfomation;
        }

        #region DI
        private readonly ILogger logger;

        private readonly ILocalContentHandler localContentHandler;

        private readonly IDomainModelConverter converter;

        private readonly IWatchPageInfomationHandler _watch;

        #endregion

        #region Methods

        public async Task<IAttemptResult<IDownloadContext>> TryDownloadContentAsync(IVideoInfo videoInfo, IDownloadSettings setting, Action<string> OnMessage, CancellationToken token)
        {
            var context = new DownloadContext(setting.NiconicoId);

            context.RegisterMessageHandler(OnMessage);

            IAttemptResult<IDomainVideoInfo> videoInfoResult = await this._watch.GetVideoInfoAsync(videoInfo.NiconicoId);

            if (!videoInfoResult.IsSucceeded || videoInfoResult.Data is null)
            {
                return AttemptResult<IDownloadContext>.Fail(videoInfoResult.Message);
            }

            IDomainVideoInfo domainVideoInfo = videoInfoResult.Data;

            this.converter.ConvertDomainVideoInfoToVideoInfo(videoInfo, videoInfoResult.Data);

            domainVideoInfo.DmcInfo.DownloadStartedOn = DateTime.Now;

            if (token.IsCancellationRequested) return this.CancelledDownloadAndGetResult();

            ILocalContentInfo? info = null;
            if (setting.Skip)
            {
                string fileNameFormat = setting.FileNameFormat;
                info = this.localContentHandler.GetLocalContentInfo(setting.FolderPath, fileNameFormat, domainVideoInfo.DmcInfo, setting.VerticalResolution, setting.IsReplaceStrictedEnable, setting.VideoInfoExt, setting.IchibaInfoExt, setting.ThumbnailExt, setting.IchibaInfoSuffix, setting.VideoInfoSuffix);
            }

            if (!Directory.Exists(setting.FolderPath))
            {
                Directory.CreateDirectory(setting.FolderPath);
            }

            //動画

            if (setting.Video)
            {

                if (info?.VideoExist ?? false)
                {
                    OnMessage("動画を保存済みのためスキップしました。");
                }
                else if (setting.FromAnotherFolder && (info?.VIdeoExistInOnotherFolder ?? false) && info?.LocalPath is not null)
                {
                    var vResult = this.localContentHandler.MoveDownloadedFile(setting.NiconicoId, info.LocalPath, setting.FolderPath);
                    if (!vResult.IsSucceeded)
                    {
                        return AttemptResult<IDownloadContext>.Fail(vResult.Message ?? "None");
                    }
                    else
                    {
                        OnMessage("別フォルダーに保存済みの動画をコピーしました。");
                    }
                }
                else
                {
                    var vResult = await this.TryDownloadVideoAsync(setting, OnMessage, domainVideoInfo, context, token);
                    if (!vResult.IsSucceeded)
                    {
                        OnMessage("DL失敗");
                        return AttemptResult<IDownloadContext>.Fail(vResult.Message ?? "None");
                    }
                }
            }

            if (token.IsCancellationRequested) return this.CancelledDownloadAndGetResult();

            //サムネイル
            if (setting.Thumbnail)
            {
                if (!info?.ThumbExist ?? true)
                {
                    var tResult = await this.TryDownloadThumbAsync(setting, domainVideoInfo);

                    if (token.IsCancellationRequested) return this.CancelledDownloadAndGetResult();

                    if (!tResult.IsSucceeded)
                    {
                        OnMessage("DL失敗");
                        return AttemptResult<IDownloadContext>.Fail(tResult.Message ?? "None");
                    }
                }
                else if (info?.ThumbExist ?? false)
                {
                    OnMessage("サムネイルを保存済みのためスキップしました。");
                }
            }

            if (token.IsCancellationRequested) return this.CancelledDownloadAndGetResult();

            //コメント
            if (setting.Comment)
            {
                if (!info?.CommentExist ?? true)
                {

                    var cResult = await this.TryDownloadCommentAsync(setting, domainVideoInfo, OnMessage, context, token);

                    if (token.IsCancellationRequested) return this.CancelledDownloadAndGetResult();

                    if (!cResult.IsSucceeded)
                    {
                        OnMessage("DL失敗");
                        return AttemptResult<IDownloadContext>.Fail(cResult.Message ?? "None");
                    }
                }
                else if (info?.CommentExist ?? false)
                {
                    OnMessage("コメントを保存済みのためスキップしました。");
                }
            }

            if (token.IsCancellationRequested) return this.CancelledDownloadAndGetResult();

            //動画情報
            if (setting.DownloadVideoInfo)
            {
                if (!info?.VideoInfoExist ?? true)
                {
                    var iResult = this.TryDownloadDescriptionAsync(setting, domainVideoInfo, OnMessage);

                    if (token.IsCancellationRequested) return this.CancelledDownloadAndGetResult();

                    if (!iResult.IsSucceeded)
                    {
                        OnMessage("DL失敗");
                        return AttemptResult<IDownloadContext>.Fail(iResult.Message ?? "None");
                    }
                }
            }

            if (token.IsCancellationRequested) return this.CancelledDownloadAndGetResult();

            //市場情報
            if (setting.DownloadIchibaInfo)
            {

                if (!info?.IchibaInfoExist ?? true)
                {
                    var iResult = await this.DownloadIchibaInfoAsync(setting, domainVideoInfo, OnMessage, context);

                    if (token.IsCancellationRequested) return this.CancelledDownloadAndGetResult();

                    if (!iResult.IsSucceeded)
                    {
                        OnMessage("DL失敗");
                        return AttemptResult<IDownloadContext>.Fail(iResult.Message ?? "None");
                    }
                }
                else
                {
                    OnMessage("市場情報を保存済みのためスキップしました。");
                }
            }

            return AttemptResult<IDownloadContext>.Succeeded(context);
        }

        #endregion

        #region private

        /// <summary>
        /// 非同期で動画をダウンロードする
        /// </summary>
        private async Task<IAttemptResult> TryDownloadVideoAsync(IDownloadSettings settings, Action<string> onMessage, IDomainVideoInfo videoInfo, IDownloadContext context, CancellationToken token)
        {
            var videoDownloader = DIFactory.Provider.GetRequiredService<Vdl::Integrate.IVideoDownloader>();
            IAttemptResult<uint> result;
            try
            {
                result = await videoDownloader.DownloadVideoAsync(settings, onMessage, videoInfo, token);
            }
            catch (Exception e)
            {
                this.logger.Error($"動画のダウンロードに失敗しました。({context.GetLogContent()})", e);
                return AttemptResult.Fail($"動画のダウンロードに失敗しました。(詳細:{e.Message})");
            }

            context.ActualVerticalResolution = result.Data;
            if (result.IsSucceeded)
            {
                return AttemptResult.Succeeded();
            } else
            {
                return AttemptResult.Fail(result.Message);
            }
        }

        /// <summary>
        /// 動画情報をダウンロードする
        /// </summary>
        private IAttemptResult TryDownloadDescriptionAsync(IDownloadSettings settings, IDomainVideoInfo videoInfo, Action<string> onMessage)
        {
            var descriptionDownloader = DIFactory.Provider.GetRequiredService<DDL::IDescriptionDownloader>();

            IAttemptResult result;
            try
            {
                result = descriptionDownloader.DownloadVideoInfoAsync(settings, videoInfo, onMessage);
            }
            catch (Exception e)
            {
                this.logger.Error($"動画情報のダウンロードに失敗しました。", e);
                return AttemptResult.Fail($"動画情報のダウンロードに失敗しました。(詳細:{e.Message})");
            }

            return result;
        }

        /// <summary>
        /// サムネイルをダウンロードする
        /// </summary>
        private async Task<IAttemptResult> TryDownloadThumbAsync(IDownloadSettings setting, IDomainVideoInfo videoInfo)
        {
            var thumbDownloader = DIFactory.Provider.GetRequiredService<Tdl::IThumbDownloader>();
            IAttemptResult result;
            try
            {
                result = await thumbDownloader.DownloadThumbnailAsync(setting, videoInfo);
            }
            catch (Exception e)
            {
                this.logger.Error($"サムネイルのダウンロードに失敗しました。", e);
                return AttemptResult.Fail($"サムネイルのダウンロードに失敗しました。(詳細:{e.Message})");
            }

            return result;
        }

        /// <summary>
        /// コメントをダウンロードする
        /// </summary>
        private async Task<IAttemptResult> TryDownloadCommentAsync(IDownloadSettings settings, IDomainVideoInfo videoInfo, Action<string> onMessage, IDownloadContext context, CancellationToken token)
        {
            //Cdl::ICommentDownloader? v1 = null;
            V2Comment::ICommentDownloader? v2 = null;

            //if (settings.EnableExperimentalCommentSafetySystem)
            //{
            v2 = DIFactory.Provider.GetRequiredService<V2Comment::ICommentDownloader>();
            //}
            //else
            //{
            //    v1 = DIFactory.Provider.GetRequiredService<Cdl::ICommentDownloader>();
            //
            //}

            IAttemptResult result;
            try
            {
                //result = v2 is null ? await v1!.DownloadComment(session, settings, onMessage, context, token) : await v2.DownloadCommentAsync(session.Video!.DmcInfo, settings, context, token);
                result = await v2.DownloadCommentAsync(videoInfo.DmcInfo, settings, context, token);
            }
            catch (Exception e)
            {
                this.logger.Error("コメントのダウンロードに失敗しました。", e);
                return AttemptResult.Fail($"コメントのダウンロードに失敗しました。({e.Message})");
            }

            return result;
        }

        /// <summary>
        /// 市場情報をダウンロードする
        /// </summary>
        private async Task<IAttemptResult> DownloadIchibaInfoAsync(IDownloadSettings settings, IDomainVideoInfo videoInfo, Action<string> onMessage, IDownloadContext context)
        {

            var iDownloader = DIFactory.Provider.GetRequiredService<IIchibaInfoDownloader>();
            IAttemptResult result;
            try
            {
                result = await iDownloader.DownloadIchibaInfo(videoInfo, settings, onMessage, context);
            }
            catch (Exception e)
            {
                this.logger.Error("市場情報のダウンロードに失敗しました。", e);
                return AttemptResult.Fail($"市場情報のダウンロードに失敗しました。({e.Message})");
            }
            return result;

        }

        /// <summary>
        /// キャンセルした結果を取得する
        /// </summary>
        private IAttemptResult<IDownloadContext> CancelledDownloadAndGetResult()
        {
            return AttemptResult<IDownloadContext>.Fail("キャンセルされました。");
        }
        #endregion
    }
}
