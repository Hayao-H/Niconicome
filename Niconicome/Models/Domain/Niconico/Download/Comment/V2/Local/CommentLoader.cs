using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Niconicome.Models.Domain.Local.IO;
using Niconicome.Models.Domain.Niconico.Net.Xml;
using Niconicome.Models.Domain.Utils;
using Niconicome.Models.Helper.Result;
using Converter = Niconicome.Models.Domain.Niconico.Download.Comment.V2.Core.Converter;
using Core = Niconicome.Models.Domain.Niconico.Download.Comment.V2.Core;
using V2 = Niconicome.Models.Domain.Niconico.Net.Xml.Comment.V2;

namespace Niconicome.Models.Domain.Niconico.Download.Comment.V2.Local
{
    public interface ICommentLoader
    {
        /// <summary>
        /// コメントを読み込む
        /// </summary>
        /// <param name="folderPath"></param>
        /// <param name="niconicoID"></param>
        /// <returns></returns>
        IAttemptResult<LocalCommentInfo> LoadComment(string folderPath, string niconicoID);

        /// <summary>
        /// コメントファイルが存在するかどうかを確かめる
        /// </summary>
        /// <param name="folderPath"></param>
        /// <param name="niconicoID"></param>
        /// <returns></returns>
        bool CommentExists(string folderPath, string niconicoID);
    }

    public class CommentLoader : ICommentLoader
    {
        public CommentLoader(Converter::ILocalCommentConverter converter, INicoDirectoryIO directoryIO, INicoFileIO fileIO, ILogger logger)
        {
            this._converter = converter;
            this._directoryIO = directoryIO;
            this._fileIO = fileIO;
            this._logger = logger;
        }

        #region field

        private readonly Converter::ILocalCommentConverter _converter;

        private readonly INicoDirectoryIO _directoryIO;

        private readonly INicoFileIO _fileIO;

        private readonly ILogger _logger;

        #endregion

        #region Method

        public IAttemptResult<LocalCommentInfo> LoadComment(string folderPath, string niconicoID)
        {
            string? path = this.GetCommentFilePath(folderPath, niconicoID);

            if (path is null)
            {
                return AttemptResult<LocalCommentInfo>.Fail("コメントファイルが存在しません。");
            }

            string content;

            try
            {
                content = this._fileIO.OpenRead(path);
            }
            catch (Exception ex)
            {
                this._logger.Error("コメントファイルの読み込みに失敗しました。", ex);
                return AttemptResult<LocalCommentInfo>.Fail($"コメントファイルの読み込みに失敗しました。（詳細:{ex.Message}）");
            }

            V2::PacketElement? data;

            try
            {
                data = Xmlparser.Deserialize<V2::PacketElement>(content);
            }
            catch (Exception ex)
            {
                this._logger.Error("コメントファイルの解析に失敗しました。", ex);
                return AttemptResult<LocalCommentInfo>.Fail($"コメントファイルの解析に失敗しました。（詳細:{ ex.Message}）");
            }

            if (data is null)
            {
                return AttemptResult<LocalCommentInfo>.Fail("コメントファイルの解析に失敗しました。");
            }

            DateTime lastUpdatedTime;

            try
            {
                var info = new FileInfo(path);
                lastUpdatedTime = info.LastWriteTime;
            }
            catch (Exception ex)
            {
                this._logger.Error("コメントファイル情報の取得に失敗しました。", ex);
                return AttemptResult<LocalCommentInfo>.Fail($"コメントファイル情報の取得に失敗しました。（詳細:{ex.Message}）");
            }

            var converted = data.Chat.Select(c => this._converter.ConvertChatToCoreComment(c));

            return AttemptResult<LocalCommentInfo>.Succeeded(new LocalCommentInfo(lastUpdatedTime, converted));



        }


        public bool CommentExists(string folderPath, string niconicoID)
        {
            return this.GetCommentFilePath(folderPath, niconicoID) is not null;
        }


        #endregion

        #region private

        /// <summary>
        /// コメントファイルのパスを取得する（存在しない場合はnullを返す）
        /// </summary>
        /// <param name="folderPath"></param>
        /// <param name="niconicoID"></param>
        /// <returns></returns>
        private string? GetCommentFilePath(string folderPath, string niconicoID)
        {
            return this._directoryIO.GetFiles(folderPath).FirstOrDefault(p => p.Contains(niconicoID));
        }

        #endregion
    }

    internal record LocalCommentInfo(DateTime LastUpdatedTime, IEnumerable<Core::IComment> Comments);
}
