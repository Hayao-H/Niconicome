using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Niconico.Watch;
using Niconicome.Models.Domain.Utils;
using Niconicome.Models.Helper.Result;
using Niconicome.Models.Network.Download;

namespace Niconicome.Models.Domain.Niconico.Download.Comment
{
    interface ICommentDownloader
    {
        /// <summary>
        /// コメントをダウンロードする
        /// </summary>
        /// <param name="session"></param>
        /// <param name="settings"></param>
        /// <param name="onMessage"></param>
        /// <param name="context"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<IAttemptResult> DownloadComment(IWatchSession session, IDownloadSettings settings, Action<string> onMessage, IDownloadContext context, CancellationToken token);
    }

    class CommentDownloader : ICommentDownloader
    {
        public CommentDownloader(IPathOrganizer pathOrganizer, ICommentClient client, ILogger logger, ICommentConverter converter, ICommentStream stream)
        {
            this._client = client;
            this._logger = logger;
            this._commentConverter = converter;
            this._commentStream = stream;
            this._pathOrganizer = pathOrganizer;
        }

        #region field

        private readonly ICommentClient _client;

        private readonly ILogger _logger;

        private readonly ICommentConverter _commentConverter;

        private readonly ICommentStream _commentStream;

        private readonly IPathOrganizer _pathOrganizer;

        #endregion

        #region Method

        public async Task<IAttemptResult> DownloadComment(IWatchSession session, IDownloadSettings settings, Action<string> onMessage, IDownloadContext context, CancellationToken token)
        {

            if (session.Video is null)
            {
                return AttemptResult.Fail("動画情報が未取得です。");
            }

            string filePath = this._pathOrganizer.GetFilePath(settings.FileNameFormat, session.Video!.DmcInfo, ".xml", settings.FolderPath, settings.IsReplaceStrictedEnable, settings.Overwrite);
            string ownerFileName = this._pathOrganizer.GetFilePath(settings.FileNameFormat, session.Video!.DmcInfo, ".xml", settings.FolderPath, settings.IsReplaceStrictedEnable, settings.Overwrite, settings.OwnerComSuffix);

            if (token.IsCancellationRequested)
            {
                return this.GetCancelledResult();
            }

            onMessage("コメントのダウンロードを開始します。");
            this._logger.Log($"{settings.NiconicoId}のコメントダウンロードを開始します。({context.GetLogContent()})");

            ICommentCollection result;
            try
            {
                result = await this._client.DownloadCommentAsync(session.Video!.DmcInfo, settings, onMessage, context, token);
            }
            catch (Exception e)
            {
                this._logger.Error($"コメントの取得に失敗しました。({context.GetLogContent()})", e);
                return AttemptResult.Fail($"コメントの取得に失敗しました。(詳細:{e.Message})");
            }
            onMessage("コメントのダウンロードが完了しました。");

            if (token.IsCancellationRequested)
            {
                return this.GetCancelledResult();
            }

            onMessage("コメントの変換処理を開始します。");
            IStoreCommentsData data;
            try
            {
                data = this._commentConverter.ConvertToStoreCommentsData(result, settings);
            }
            catch (Exception e)
            {
                this._logger.Error($"コメントの解析に失敗しました。({context.GetLogContent()})", e);
                return AttemptResult.Fail($"コメントの解析に失敗しました。");
            }
            onMessage("コメントの変換処理が完了しました。");

            data.FilePath = filePath;
            data.OwnerFilPath = ownerFileName;

            if (token.IsCancellationRequested)
            {
                return this.GetCancelledResult();
            }

            onMessage($"コメントの書き込みを開始します。");
            try
            {
                this._commentStream.Write(data, settings.Overwrite);
            }
            catch (Exception e)
            {
                this._logger.Error($"コメントの書き込みに失敗しました。({context.GetLogContent()})", e);
                return AttemptResult.Fail("コメントの書き込みに失敗しました。");
            }
            onMessage("コメントのダウンロードが完了しました。");
            this._logger.Log($"コメントのダウンロードが完了しました。({context.GetLogContent()})");

            return AttemptResult.Succeeded();


        }

        #endregion

        #region private

        private IAttemptResult GetCancelledResult()
        {
            return AttemptResult.Fail("処理がキャンセルされました");
        }

        #endregion
    }
}
