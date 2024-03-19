using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Niconicome.Models.Const;
using Niconicome.Models.Domain.Local.IO.V2;
using Niconicome.Models.Domain.Niconico.Download;
using Niconicome.Models.Domain.Niconico.Download.General;
using Niconicome.Models.Domain.Niconico.Download.Ichiba;
using Niconicome.Models.Domain.Niconico.Video.Infomations;
using Niconicome.Models.Domain.Niconico.Watch.V2;
using Niconicome.Models.Domain.Playlist;
using Niconicome.Models.Domain.Utils;
using Niconicome.Models.Helper.Result;
using Niconicome.Models.Network.Watch;
using DDL = Niconicome.Models.Domain.Niconico.Download.Description;
using Tdl = Niconicome.Models.Domain.Niconico.Download.Thumbnail;
using V2Comment = Niconicome.Models.Domain.Niconico.Download.Comment.V2.Integrate;
using Vdl = Niconicome.Models.Domain.Niconico.Download.Video.V3;
using VdlLegacy = Niconicome.Models.Domain.Niconico.Download.Video.V2;

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
        public ContentDownloadHelper(ILogger logger, IDomainModelConverter converter, IWatchPageInfomationHandler watchPageInfomation, ILocalFileHandler localFileHandler, IPathOrganizer pathOrganizer, INiconicomeDirectoryIO directoryIO)
        {
            this.converter = converter;
            this.logger = logger;
            this._watch = watchPageInfomation;
            this._localFileHandler = localFileHandler;
            this._pathOrganizer = pathOrganizer;
            this._directoryIO = directoryIO;
        }

        #region DI
        private readonly ILogger logger;

        private readonly ILocalFileHandler _localFileHandler;

        private readonly IDomainModelConverter converter;

        private readonly IWatchPageInfomationHandler _watch;

        private readonly IPathOrganizer _pathOrganizer;

        private readonly INiconicomeDirectoryIO _directoryIO;

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

            ILocalFileInfo? info = null;

            if (setting.Skip)
            {
                IAttemptResult<ILocalFileInfo> localResult = this._localFileHandler.GetLocalContentInfo(setting.FolderPath, videoInfo.NiconicoId, false, setting.VerticalResolution, setting.ThumbnailExt, setting.IchibaInfoSuffix, setting.VideoInfoSuffix, setting.EconomySuffix);

                if (localResult.IsSucceeded && localResult.Data is not null)
                {
                    info = localResult.Data;
                }
            }


            string directoryName = Path.GetDirectoryName(this.GetVideoFilePath(setting, domainVideoInfo)) ?? string.Empty;
            if (!this._directoryIO.Exists(directoryName))
            {
                IAttemptResult dResult = this._directoryIO.CreateDirectory(directoryName);
                if (!dResult.IsSucceeded)
                {
                    return AttemptResult<IDownloadContext>.Fail(dResult.Message ?? "None");
                }
            }

            //動画

            if (setting.Video)
            {
                if (info is not null && info.VideoExists)
                {
                    OnMessage("動画を保存済みのためスキップしました。");
                }
                else if (info is not null && setting.FromAnotherFolder && info.VideoExistInOnotherFolder)
                {
                    string path = this.GetVideoFilePath(setting, domainVideoInfo);
                    IAttemptResult vResult = this._localFileHandler.MoveDownloadedVideoFile(info.VideoFilePath, path);
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
                if (info is not null && info.ThumbExists)
                {
                    OnMessage("サムネイルを保存済みのためスキップしました。");
                }
                else
                {
                    var tResult = await this.TryDownloadThumbAsync(setting, domainVideoInfo);

                    if (token.IsCancellationRequested) return this.CancelledDownloadAndGetResult();

                    if (!tResult.IsSucceeded)
                    {
                        OnMessage("DL失敗");
                        return AttemptResult<IDownloadContext>.Fail(tResult.Message ?? "None");
                    }
                }
            }

            if (token.IsCancellationRequested) return this.CancelledDownloadAndGetResult();

            //コメント
            if (setting.Comment)
            {
                if (info is not null && info.CommentExists)
                {
                    OnMessage("コメントを保存済みのためスキップしました。");
                }
                else
                {

                    var cResult = await this.TryDownloadCommentAsync(setting, domainVideoInfo, OnMessage, context, token);

                    if (token.IsCancellationRequested) return this.CancelledDownloadAndGetResult();

                    if (!cResult.IsSucceeded)
                    {
                        OnMessage("DL失敗");
                        return AttemptResult<IDownloadContext>.Fail(cResult.Message ?? "None");
                    }
                }
            }

            if (token.IsCancellationRequested) return this.CancelledDownloadAndGetResult();

            //動画情報
            if (setting.DownloadVideoInfo)
            {
                if (info is not null && info.VideoInfoExist)
                {
                    OnMessage("動画情報を保存済みのためスキップしました。");
                }
                else
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
                if (info is not null && info.IchibaInfoExist)
                {
                    OnMessage("市場情報を保存済みのためスキップしました。");
                }
                else
                {
                    var iResult = await this.DownloadIchibaInfoAsync(setting, domainVideoInfo, OnMessage, context);

                    if (token.IsCancellationRequested) return this.CancelledDownloadAndGetResult();

                    if (!iResult.IsSucceeded)
                    {
                        OnMessage("DL失敗");
                        return AttemptResult<IDownloadContext>.Fail(iResult.Message ?? "None");
                    }
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
            IAttemptResult<uint> result;
            try
            {
                if (videoInfo.DmcInfo.IsDMS)
                {
                    IAttemptResult<int> resultNew = await DIFactory.Provider.GetRequiredService<Vdl::Integrate.IVideoDownloader>().DownloadVideoAsync(settings, onMessage, videoInfo, token);
                    result = resultNew.IsSucceeded ? AttemptResult<uint>.Succeeded((uint)resultNew.Data) : AttemptResult<uint>.Fail(resultNew.Message);
                }
                else
                {
                    result = await DIFactory.Provider.GetRequiredService<VdlLegacy.Integrate.IVideoDownloader>().DownloadVideoAsync(settings, onMessage, videoInfo, token);
                }
            }
            catch (Exception e)
            {
                this.logger.Error($"動画のダウンロードに失敗しました。({context.GetLogContent()})", e);
                return AttemptResult.Fail($"動画のダウンロードに失敗しました。(詳細:{e.Message})");
            }

            context.ActualVerticalResolution = (uint)result.Data;
            if (result.IsSucceeded)
            {
                return AttemptResult.Succeeded();
            }
            else
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

        /// <summary>
        /// 動画の保存先を取得する
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="videoInfo"></param>
        /// <returns></returns>
        private string GetVideoFilePath(IDownloadSettings settings, IDomainVideoInfo videoInfo)
        {
            string path = this._pathOrganizer.GetFilePath(settings.FileNameFormat, videoInfo.DmcInfo, settings.SaveWithoutEncode ? FileFolder.TsFileExt : FileFolder.Mp4FileExt, settings.FolderPath, settings.IsReplaceStrictedEnable, settings.Overwrite);
            return path;
        }
        #endregion
    }
}
