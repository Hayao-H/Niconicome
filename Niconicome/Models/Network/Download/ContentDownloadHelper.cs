using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Niconicome.Models.Domain.Niconico.Download;
using Niconicome.Models.Domain.Niconico.Watch;
using Niconicome.Models.Domain.Utils;
using Niconicome.Models.Local.Settings;
using Niconicome.Models.Network.Watch;
using Cdl = Niconicome.Models.Domain.Niconico.Download.Comment;
using DDL = Niconicome.Models.Domain.Niconico.Download.Description;
using Download = Niconicome.Models.Domain.Niconico.Download;
using Tdl = Niconicome.Models.Domain.Niconico.Download.Thumbnail;
using Vdl = Niconicome.Models.Domain.Niconico.Download.Video;
using EnumSetting = Niconicome.Models.Local.Settings.EnumSettingsValue;
using Niconicome.Models.Helper.Result;
using Niconicome.Models.Local.Settings.EnumSettingsValue;
using Niconicome.Models.Domain.Niconico.Download.Ichiba;
using Niconicome.Models.Playlist;

namespace Niconicome.Models.Network.Download
{
    interface IContentDownloadHelper
    {
        INetworkResult? CurrentResult { get; }

        Task<IDownloadResult> TryDownloadContentAsync(IDownloadSettings setting, Action<string> OnMessage, CancellationToken token);
    }

    class ContentDownloadHelper : IContentDownloadHelper
    {
        public ContentDownloadHelper(ILogger logger, ILocalContentHandler localContentHandler, ILocalSettingHandler localSettingHandler, IDomainModelConverter converter, IEnumSettingsHandler enumSettingsHander, IPathOrganizer pathOrganizer, IVideoInfoContainer container)
        {
            this.localContentHandler = localContentHandler;
            this.settingHandler = localSettingHandler;
            this.converter = converter;
            this.logger = logger;
            this.enumSettingsHandler = enumSettingsHander;
            this.pathOrganizer = pathOrganizer;
            this.container = container;
        }

        #region DI
        private readonly ILogger logger;

        private readonly ILocalSettingHandler settingHandler;

        private readonly ILocalContentHandler localContentHandler;

        private readonly IDomainModelConverter converter;

        private readonly IEnumSettingsHandler enumSettingsHandler;

        private readonly IPathOrganizer pathOrganizer;

        private readonly IVideoInfoContainer container;

        #endregion

        /// <summary>
        /// 現在の結果
        /// </summary>
        public INetworkResult? CurrentResult { get; private set; }

        /// <summary>
        /// 非同期でコンテンツをダウンロードする
        /// </summary>
        /// <param name="setting"></param>
        /// <param name="OnMessage"></param>
        /// <returns></returns>
        public async Task<IDownloadResult> TryDownloadContentAsync(IDownloadSettings setting, Action<string> OnMessage, CancellationToken token)
        {
            var result = new DownloadResult() { VideoInfo = this.container.GetVideo(setting.NiconicoId) };
            var context = new DownloadContext(setting.NiconicoId);
            var session = DIFactory.Provider.GetRequiredService<IWatchSession>();

            await session.GetVideoDataAsync(setting.NiconicoId, setting.Video);

            if (session.Video is not null)
            {
                this.converter.ConvertDomainVideoInfoToListVideoInfo(result.VideoInfo, session.Video);
            }


            if (session.Video?.DmcInfo.DownloadStartedOn is not null)
            {
                session.Video.DmcInfo.DownloadStartedOn = DateTime.Now;
            }

            if (session.State != WatchSessionState.GotPage || session.Video is null)
            {
                result.IsSucceeded = false;
                result.Message = session.State switch
                {
                    WatchSessionState.PaymentNeeded => "視聴ページの解析に失敗しました。",
                    WatchSessionState.HttpRequestOrPageAnalyzingFailure => "視聴ページの取得、または視聴ページの解析に失敗しました。",
                    _ => "不明なエラーにより、視聴ページの取得に失敗しました。"
                };
                return result;
            }

            if (token.IsCancellationRequested) return this.CancelledDownloadAndGetResult();

            ILocalContentInfo? info = null;
            if (setting.Skip)
            {
                string fileNameFormat = setting.FileNameFormat;
                info = this.localContentHandler.GetLocalContentInfo(setting.FolderPath, fileNameFormat, session.Video.DmcInfo, setting.IsReplaceStrictedEnable, setting.VideoInfoExt, setting.IchibaInfoExt, setting.ThumbnailExt, setting.IchibaInfoSuffix, setting.VideoInfoSuffix);
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
                    result.IsSucceeded = true;
                }
                else if (setting.FromAnotherFolder && (info?.VIdeoExistInOnotherFolder ?? false) && info?.LocalPath is not null)
                {
                    var vResult = this.localContentHandler.MoveDownloadedFile(setting.NiconicoId, info.LocalPath, setting.FolderPath);
                    if (!vResult.IsSucceeded)
                    {
                        result.IsSucceeded = false;
                        result.Message = vResult.Message ?? "None";
                        return result;
                    }
                    else
                    {
                        result.IsSucceeded = true;
                        result.VideoFileName = vResult.VideoFileName ?? string.Empty;
                        OnMessage("別フォルダーに保存済みの動画をコピーしました。");
                    }
                }
                else
                {
                    var vResult = await this.TryDownloadVideoAsync(setting, OnMessage, session, context, token);
                    if (!vResult.IsSucceeded)
                    {
                        result.IsSucceeded = false;
                        result.Message = vResult.Message ?? "None";
                        OnMessage("DL失敗");
                        return result;
                    }
                    else
                    {
                        result.IsSucceeded = true;
                        result.VideoFileName = vResult.VideoFileName ?? string.Empty;
                        result.VideoInfo.FileName.Value = vResult.VideoFileName ?? string.Empty;
                        result.VideoVerticalResolution = vResult.VideoVerticalResolution;
                    }
                }
            }

            if (token.IsCancellationRequested) return this.CancelledDownloadAndGetResult();

            //サムネイル
            if (setting.Thumbnail)
            {
                if (!info?.ThumbExist ?? true)
                {
                    var tResult = await this.TryDownloadThumbAsync(setting, session);

                    if (token.IsCancellationRequested) return this.CancelledDownloadAndGetResult();

                    if (!tResult.IsSucceeded)
                    {
                        result.IsSucceeded = false;
                        result.Message = tResult.Message ?? "None";
                        OnMessage("DL失敗");
                        return result;
                    }
                    else
                    {
                        result.IsSucceeded = true;
                    }
                }
                else if (info?.ThumbExist ?? false)
                {
                    OnMessage("サムネイルを保存済みのためスキップしました。");
                    result.IsSucceeded = true;
                }
            }

            if (token.IsCancellationRequested) return this.CancelledDownloadAndGetResult();

            //コメント
            if (setting.Comment)
            {
                if (!info?.CommentExist ?? true)
                {

                    var cResult = await this.TryDownloadCommentAsync(setting, session, OnMessage, context, token);

                    if (token.IsCancellationRequested) return this.CancelledDownloadAndGetResult();

                    if (!cResult.IsSucceeded)
                    {
                        result.IsSucceeded = false;
                        result.Message = cResult.Message ?? "None";
                        OnMessage("DL失敗");
                        return result;
                    }
                    else
                    {
                        result.IsSucceeded = true;
                    }
                }
                else if (info?.CommentExist ?? false)
                {
                    OnMessage("コメントを保存済みのためスキップしました。");
                    result.IsSucceeded = true;
                }
            }

            if (token.IsCancellationRequested) return this.CancelledDownloadAndGetResult();

            //動画情報
            if (setting.DownloadVideoInfo)
            {
                if (!info?.VideoInfoExist ?? true)
                {
                    var iResult = this.TryDownloadDescriptionAsync(setting, session, OnMessage);

                    if (token.IsCancellationRequested) return this.CancelledDownloadAndGetResult();

                    if (!iResult.IsSucceeded)
                    {
                        result.IsSucceeded = false;
                        result.Message = iResult.Message ?? "None";
                        OnMessage("DL失敗");
                        return result;
                    }
                    else
                    {
                        result.IsSucceeded = true;
                    }
                }
            }

            if (token.IsCancellationRequested) return this.CancelledDownloadAndGetResult();

            //市場情報
            if (setting.DownloadIchibaInfo)
            {

                if (!info?.IchibaInfoExist ?? true)
                {
                    var iResult = await this.DownloadIchibaInfoAsync(setting, session, OnMessage, context);

                    if (token.IsCancellationRequested) return this.CancelledDownloadAndGetResult();

                    if (!iResult.IsSucceeded)
                    {
                        result.IsSucceeded = false;
                        result.Message = iResult.Message ?? "None";
                        OnMessage("DL失敗");
                        return result;
                    }
                    else
                    {
                        result.IsSucceeded = true;
                    }
                }
                else
                {
                    OnMessage("市場情報を保存済みのためスキップしました。");
                    result.IsSucceeded = true;
                }
            }

            if (session.IsSessionEnsured) session.Dispose();

            return result;
        }

        #region private

        /// <summary>
        /// 非同期で動画をダウンロードする
        /// </summary>
        /// <param name="niconicoid"></param>
        /// <param name="onMessage"></param>
        /// <returns></returns>
        private async Task<IDownloadResult> TryDownloadVideoAsync(IDownloadSettings settings, Action<string> onMessage, IWatchSession session, IDownloadContext context, CancellationToken token)
        {
            int maxParallel = this.settingHandler.GetIntSetting(SettingsEnum.MaxParallelSegDl);
            if (maxParallel <= 0)
            {
                maxParallel = 1;
            }

            var vSettings = settings.ConvertToVideoDownloadSettings(false, maxParallel);
            var videoDownloader = DIFactory.Provider.GetRequiredService<Vdl::IVideoDownloader>();
            Download::IDownloadResult result;
            try
            {
                result = await videoDownloader.DownloadVideoAsync(vSettings, onMessage, session, context, token);
            }
            catch (Exception e)
            {
                this.logger.Error("動画のダウンロードに失敗しました。", e);
                return new DownloadResult() { IsSucceeded = false, Message = e.Message };
            }
            return new DownloadResult() { IsSucceeded = result.Issucceeded, Message = result.Message ?? null, VideoFileName = result.VideoFileName, VideoVerticalResolution = result.VerticalResolution };
        }

        /// <summary>
        /// 動画情報をダウンロードする
        /// </summary>
        /// <param name="niconicoid"></param>
        /// <param name="onMessage"></param>
        /// <returns></returns>
        private IDownloadResult TryDownloadDescriptionAsync(IDownloadSettings settings, IWatchSession session, Action<string> onMessage)
        {
            var dlType = this.enumSettingsHandler.GetSetting<EnumSetting::VideoInfoTypeSettings>();

            var dSettings = settings.ConvertToDescriptionDownloadSetting(dlType == EnumSetting::VideoInfoTypeSettings.Json, dlType == EnumSetting::VideoInfoTypeSettings.Xml, dlType == EnumSetting::VideoInfoTypeSettings.Text);
            var descriptionDownloader = DIFactory.Provider.GetRequiredService<DDL::IDescriptionDownloader>();
            Download::IDownloadResult result;
            try
            {
                result = descriptionDownloader.DownloadVideoInfo(dSettings, session, onMessage);
            }
            catch (Exception e)
            {
                this.logger.Error("動画情報のダウンロードに失敗しました。", e);
                return new DownloadResult() { IsSucceeded = false, Message = e.Message };
            }
            return new DownloadResult() { IsSucceeded = result.Issucceeded, Message = result.Message ?? null, VideoFileName = result.VideoFileName, VideoVerticalResolution = result.VerticalResolution };
        }

        /// <summary>
        /// サムネイルをダウンロードする
        /// </summary>
        /// <param name="setting"></param>
        /// <param name="session"></param>
        /// <returns></returns>
        private async Task<IDownloadResult> TryDownloadThumbAsync(IDownloadSettings setting, IWatchSession session)
        {
            var tSettings = setting.ConvertToThumbDownloadSetting();
            var thumbDownloader = DIFactory.Provider.GetRequiredService<Tdl::IThumbDownloader>();
            Download::IDownloadResult result;
            try
            {
                result = await thumbDownloader.DownloadThumbnailAsync(tSettings, session);
            }
            catch (Exception e)
            {
                this.logger.Error("サムネイルのダウンロードに失敗しました。", e);
                return new DownloadResult() { IsSucceeded = false, Message = e.Message };
            }
            return new DownloadResult() { IsSucceeded = result.Issucceeded, Message = result.Message ?? null };
        }

        /// <summary>
        /// コメントをダウンロードする
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="session"></param>
        /// <param name="onMessage"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        private async Task<IDownloadResult> TryDownloadCommentAsync(IDownloadSettings settings, IWatchSession session, Action<string> onMessage, IDownloadContext context, CancellationToken token)
        {
            var cOffset = this.settingHandler.GetIntSetting(SettingsEnum.CommentOffset);
            var autoSwicth = this.settingHandler.GetBoolSetting(SettingsEnum.SwitchOffset);

            if (cOffset < 0) cOffset = Cdl::CommentCollection.NumberToThrough;
            if (autoSwicth && session.Video!.DmcInfo.IsOfficial) cOffset = 0;

            var cSettings = settings.ConvertToCommentDownloadSetting(cOffset);
            var commentDownloader = DIFactory.Provider.GetRequiredService<Cdl::ICommentDownloader>();
            Download::IDownloadResult result;
            try
            {
                result = await commentDownloader.DownloadComment(session, cSettings, onMessage, context, token);
            }
            catch (Exception e)
            {
                this.logger.Error("コメントのダウンロードに失敗しました。", e);
                return new DownloadResult() { IsSucceeded = false, Message = e.Message };
            }
            return new DownloadResult() { IsSucceeded = result.Issucceeded, Message = result.Message ?? null };
        }

        /// <summary>
        /// 市場情報をダウンロードする
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="session"></param>
        /// <param name="onMessage"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        private async Task<IAttemptResult> DownloadIchibaInfoAsync(IDownloadSettings settings, IWatchSession session, Action<string> onMessage, IDownloadContext context)
        {
            var iSettings = settings.ConvertToIchibaInfoDownloadSettings();
            var filePath = this.pathOrganizer.GetFIlePath(settings.FileNameFormat, session.Video!.DmcInfo, settings.IchibaInfoExt, settings.FolderPath, settings.IsReplaceStrictedEnable, settings.Overwrite, settings.IchibaInfoSuffix);
            IOUtils.CreateDirectoryIfNotExist(filePath);
            iSettings.FilePath = filePath;

            var iDownloader = DIFactory.Provider.GetRequiredService<IIchibaInfoDownloader>();
            var result = await iDownloader.DownloadIchibaInfo(session, iSettings, onMessage, context);
            return result;

        }

        /// <summary>
        /// DLをキャンセルする
        /// </summary>
        /// <returns></returns>
        private IDownloadResult CancelledDownloadAndGetResult()
        {

            return new DownloadResult() { Message = "キャンセルされました。", IsCanceled = true };
        }
        #endregion
    }
}
