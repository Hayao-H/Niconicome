using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Niconicome.Models.Domain.Local.IO;
using Niconicome.Models.Domain.Niconico.Net.Xml;
using Niconicome.Models.Domain.Utils;
using Niconicome.Models.Helper.Result;
using Converter = Niconicome.Models.Domain.Niconico.Download.Comment.V2.Core.Converter;
using Core = Niconicome.Models.Domain.Niconico.Download.Comment.V2.Core;
using V2 = Niconicome.Models.Domain.Niconico.Net.Xml.Comment.V2;

namespace Niconicome.Models.Domain.Niconico.Download.Comment.V2.Local
{
    public interface ICommentWriter
    {
        /// <summary>
        /// コメントを書き込む
        /// </summary>
        /// <param name="comments"></param>
        /// <param name="threadInfo"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        IAttemptResult WriteComment(IEnumerable<Core::IComment> comments, Core::IThreadInfo threadInfo, CommentWriterOption option);
    }

    public class CommentWriter : ICommentWriter
    {
        public CommentWriter(INicoFileIO fileIO, ILogger logger, Converter::ILocalCommentConverter converter)
        {
            this._fileIO = fileIO;
            this._logger = logger;
            this._converter = converter;
        }

        #region field

        private readonly INicoFileIO _fileIO;

        private readonly ILogger _logger;

        private readonly Converter::ILocalCommentConverter _converter;

        #endregion

        #region Method

        public IAttemptResult WriteComment(IEnumerable<Core::IComment> comments, Core::IThreadInfo threadInfo, CommentWriterOption option)
        {
            string content;

            var data = new V2::PacketElement() { VideoID = option.VideoID };
            data.Chat.AddRange(comments.Select(c => this._converter.ConvertCoreCommentToChat(c)));
            data.Thread = this._converter.ConvertCoreThreadInfoToThread(threadInfo);

            try
            {
                content = Xmlparser.Serialize(data, new XmlWriterSettings() { OmitXmlDeclaration = option.OmitXmlDeclaration, Indent = true });
            }
            catch (Exception ex)
            {
                this._logger.Error("コメントのシリアライズに失敗しました。", ex);
                return AttemptResult.Fail($"コメントのシリアライズに失敗しました。（詳細:{ex}）");
            }

            try
            {
                this._fileIO.Write(option.Path, content);
            }
            catch (Exception ex)
            {
                this._logger.Error("コメントの書き込みに失敗しました。", ex);
                return AttemptResult.Fail($"コメントの書き込みに失敗しました。（詳細:{ex}）");
            }

            return AttemptResult.Succeeded();
        }


        #endregion
    }

    public record CommentWriterOption(string Path, bool OmitXmlDeclaration, string VideoID);
}
