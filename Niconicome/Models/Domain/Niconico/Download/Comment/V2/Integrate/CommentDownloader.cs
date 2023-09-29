using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Niconico.Video.Infomations;
using Niconicome.Models.Domain.Utils;
using Niconicome.Models.Helper.Result;
using Niconicome.Models.Network.Download;
using Core = Niconicome.Models.Domain.Niconico.Download.Comment.V2.Core;
using Fetch = Niconicome.Models.Domain.Niconico.Download.Comment.V2.Fetch;
using FetchV2 = Niconicome.Models.Domain.Niconico.Download.Comment.V2.Fetch.V3;
using Local = Niconicome.Models.Domain.Niconico.Download.Comment.V2.Local;

namespace Niconicome.Models.Domain.Niconico.Download.Comment.V2.Integrate
{
    public interface ICommentDownloader
    {
        /// <summary>
        /// 非同期にコメントをダウンロードして保存する
        /// </summary>
        /// <param name="dmcInfo"></param>
        /// <param name="settings"></param>
        /// <param name="context"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<IAttemptResult> DownloadCommentAsync(IDmcInfo dmcInfo, IDownloadSettings settings, IDownloadContext context, CancellationToken token);
    }

    public class CommentDownloader : ICommentDownloader
    {
        public CommentDownloader(IPathOrganizer path, Fetch::ICommentClient commentClient, Local::ICommentLoader commentLoader, Local::ICommentWriter commentWriter, FetchV2::ICommentClient commentClientV2)
        {
            this._path = path;
            this._commentClient = commentClient;
            this._commentClientV2 = commentClientV2;
            this._commentWriter = commentWriter;
            this._commentLoader = commentLoader;
        }

        #region field

        private readonly IPathOrganizer _path;

        private readonly Fetch::ICommentClient _commentClient;

        private readonly FetchV2::ICommentClient _commentClientV2;

        private readonly Local::ICommentLoader _commentLoader;

        private readonly Local::ICommentWriter _commentWriter;

        #endregion

        #region Method

        public async Task<IAttemptResult> DownloadCommentAsync(IDmcInfo dmcInfo, IDownloadSettings settings, IDownloadContext context, CancellationToken token)
        {
            //コメント追記処理
            var origination = DateTime.Now;
            var originationSpecidied = false;
            var oldComments = new List<Core::IComment>();

            if (settings.AppendingToLocalComment && this._commentLoader.CommentExists(settings.FolderPath, settings.NiconicoId))
            {
                IAttemptResult<Local::LocalCommentInfo> localResult = this._commentLoader.LoadComment(settings.FolderPath, settings.NiconicoId);
                if (localResult.IsSucceeded && localResult.Data is not null)
                {
                    origination = localResult.Data.LastUpdatedTime;
                    originationSpecidied = true;
                    oldComments.AddRange(localResult.Data.Comments);
                }
            }

            //キャンセル処理
            token.ThrowIfCancellationRequested();

            //コメント取得処理
            var dlOption = new Fetch::CommentClientOption(originationSpecidied, settings.MaxCommentsCount > 0, origination, settings.MaxCommentsCount);

            IAttemptResult<Core::ICommentCollectionShared> dlResult = settings.EnableExperimentalCommentSafetySystem ? await this._commentClientV2.DownloadCommentAsync(dmcInfo, settings, dlOption, context, token) : await this._commentClient.DownloadCommentAsync(dmcInfo, settings, dlOption, context, token);

            if (!dlResult.IsSucceeded || dlResult.Data is null)
            {
                return AttemptResult.Fail(dlResult.Message);
            }

            //コメント統合処理
            var collection = dlResult.Data;
            if (oldComments.Count > 0)
            {
                foreach (var comment in oldComments) collection.Add(comment);
            }

            //キャンセル処理
            token.ThrowIfCancellationRequested();


            //コメント書き込み処理
            string path = this._path.GetFilePath(settings.FileNameFormat, dmcInfo, ".xml", settings.FolderPath, settings.IsReplaceStrictedEnable, settings.Overwrite);
            var writerOption = new Local::CommentWriterOption(path, settings.OmittingXmlDeclaration, dmcInfo.Id);

            IAttemptResult writeResult = this._commentWriter.WriteComment(collection.Comments, writerOption);

            if (!writeResult.IsSucceeded)
            {
                return writeResult;
            }

            //キャンセル処理
            token.ThrowIfCancellationRequested();

            //コメ数記録
            context.CommentCount = collection.Count;

            return AttemptResult.Succeeded();

        }

        #endregion

    }
}
