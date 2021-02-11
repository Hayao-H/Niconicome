using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Niconico.Watch;
using Niconicome.Models.Domain.Utils;

namespace Niconicome.Models.Domain.Niconico.Download.Comment
{
    interface ICommentDownloader
    {
        Task<IDownloadResult> DownloadComment(IWatchSession session, ICommentDownloadSettings settings, Action<string> onMessage, CancellationToken token);
    }

    class CommentDownloader : ICommentDownloader
    {
        public CommentDownloader(INiconicoUtils utils, ICommentClient client, IDownloadMessenger messenger, ILogger logger, ICommentConverter converter, ICommentStream stream)
        {
            this.utils = utils;
            this.client = client;
            this.messenger = messenger;
            this.logger = logger;
            this.commentConverter = converter;
            this.commentStream = stream;
        }

        private readonly INiconicoUtils utils;

        private readonly ICommentClient client;

        private readonly IDownloadMessenger messenger;

        private readonly ILogger logger;

        private readonly ICommentConverter commentConverter;

        private readonly ICommentStream commentStream;

        /// <summary>
        /// コメントをダウンロードする
        /// </summary>
        /// <param name="session"></param>
        /// <param name="settings"></param>
        /// <param name="onMessage"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<IDownloadResult> DownloadComment(IWatchSession session, ICommentDownloadSettings settings, Action<string> onMessage, CancellationToken token)
        {
            this.messenger.AddHandler(onMessage);

            if (session.IsSessionExipired)
            {
                this.messenger.RemoveHandler(onMessage);
                return new DownloadResult()
                {
                    Message = "セッションが失効しています。"
                };
            }

            if (!session.IsSessionEnsured)
            {
                try
                {
                    await session.EnsureSessionAsync(settings.NiconicoId);
                }
                catch (Exception e)
                {
                    this.messenger.RemoveHandler(onMessage);
                    this.logger.Error("セッションの確立に失敗しました。", e);
                    return new DownloadResult()
                    {
                        Message = $"セッションの確立に失敗しました。(詳細:{e.Message})"
                    };
                }
            }

            string fileName = this.utils.GetFileName(settings.FileNameFormat, session.Video!.DmcInfo, ".xml");
            string ownerFileName = this.utils.GetFileName(settings.FileNameFormat, session.Video!.DmcInfo, ".xml", "[owner]");

            if (token.IsCancellationRequested)
            {
                this.messenger.RemoveHandler(onMessage);
                return this.GetCancelledResult();
            }

            this.messenger.SendMessage("コメントのダウンロードを開始します。");
            this.logger.Log($"{settings.NiconicoId}のコメントダウンロードを開始します。");

            ICommentCollection result;
            try
            {
                result = await this.client.DownloadCommentInternalAsync(session.Video!.DmcInfo, settings, this.messenger, token);
            }
            catch (Exception e)
            {
                this.messenger.RemoveHandler(onMessage);
                this.logger.Error("コメントの取得に失敗しました。", e);
                return new DownloadResult()
                {
                    Message = $"コメントの取得に失敗しました。(詳細:{e.Message})"
                };
            }
            this.messenger.SendMessage("コメントのダウンロードが完了しました。");

            if (token.IsCancellationRequested)
            {
                this.messenger.RemoveHandler(onMessage);
                return this.GetCancelledResult();
            }

            this.messenger.SendMessage("コメントの変換処理を開始します。");
            IStoreCommentsData data;
            try
            {
                data = this.commentConverter.ConvertToStoreCommentsData(result, settings);
            }
            catch (Exception e)
            {
                this.messenger.RemoveHandler(onMessage);
                this.logger.Error("コメントの解析に失敗しました。", e);
                return new DownloadResult()
                {
                    Message = $"コメントの解析に失敗しました。(詳細:{e.Message})"
                };
            }
            this.messenger.SendMessage("コメントの変換処理が完了しました。");

            data.Filename = fileName;
            data.OwnerFilename = ownerFileName;

            if (token.IsCancellationRequested)
            {
                this.messenger.RemoveHandler(onMessage);
                return this.GetCancelledResult();
            }

            this.messenger.SendMessage("コメントの書き込みを開始します。");
            try
            {
                this.commentStream.Write(data, settings.FolderName, settings.IsOverwriteEnable);
            }
            catch (Exception e)
            {
                this.messenger.RemoveHandler(onMessage);
                this.logger.Error("コメントの書き込みに失敗しました。", e);
                return new DownloadResult()
                {
                    Message = $"コメントの書き込みに失敗しました。(詳細:{e.Message})"
                };
            }
            this.messenger.SendMessage("コメントのダウンロードが完了しました。");
            this.logger.Log("コメントのダウンロードが完了しました。");

            this.messenger.RemoveHandler(onMessage);
            return new DownloadResult()
            {
                Issucceeded = true
            };


        }

        /// <summary>
        /// キャンセル時の結果を取得する
        /// </summary>
        /// <returns></returns>
        private IDownloadResult GetCancelledResult()
        {
            return new DownloadResult()
            {
                Message = "処理がキャンセルされました"
            };
        }
    }
}
