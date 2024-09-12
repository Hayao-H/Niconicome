using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Local.IO.V2;
using Niconicome.Models.Domain.Local.Server.API.Comment.V1.Error;
using Niconicome.Models.Domain.Niconico.Net.Xml;
using Niconicome.Models.Domain.Utils.Error;
using Niconicome.Models.Helper.Result;
using CXml = Niconicome.Models.Domain.Niconico.Net.Xml.Comment.V2;

namespace Niconicome.Models.Domain.Local.Server.API.Comment.V1.Local
{
    public interface ICommentRetreiver
    {
        IAttemptResult<CommentType> GetComment(string niconicoID, string folderPath);
    }

    internal class CommentRetreiver : ICommentRetreiver
    {
        public CommentRetreiver(INiconicomeDirectoryIO directoryIO, INiconicomeFileIO fileIO, IErrorHandler errorHandler, ICommentConverter converter)
        {
            this._directoryIO = directoryIO;
            this._fileIO = fileIO;
            this._errorHandler = errorHandler;
            this._converter = converter;
        }

        #region field

        private readonly INiconicomeDirectoryIO _directoryIO;

        private readonly INiconicomeFileIO _fileIO;

        private readonly IErrorHandler _errorHandler;

        private readonly ICommentConverter _converter;

        #endregion

        public IAttemptResult<CommentType> GetComment(string niconicoID, string folderPath)
        {
            IAttemptResult<CXml::PacketElement> commentResult = this.GetXmlComment(niconicoID, folderPath);
            if (!commentResult.IsSucceeded || commentResult.Data is null)
            {
                return AttemptResult<CommentType>.Fail(commentResult.Message);
            }

            CommentType converted = this._converter.Convert(commentResult.Data);

            return AttemptResult<CommentType>.Succeeded(converted);
        }


        private IAttemptResult<CXml::PacketElement> GetXmlComment(string niconicoID, string folderPath)
        {
            IAttemptResult<IEnumerable<string>> fileResult = this._directoryIO.GetFiles(folderPath, "*.xml");
            if (!fileResult.IsSucceeded || fileResult.Data is null)
            {
                return AttemptResult<CXml::PacketElement>.Fail(fileResult.Message);
            }

            string? targetFile = fileResult.Data.FirstOrDefault(f => f.Contains(niconicoID));
            if (targetFile is null)
            {
                return AttemptResult<CXml::PacketElement>.Fail(this._errorHandler.HandleError(CommentRetreiverError.CommentFileNotFound));
            }

            if (!this._fileIO.Exists(targetFile))
            {
                return AttemptResult<CXml::PacketElement>.Fail(this._errorHandler.HandleError(CommentRetreiverError.CommentFileNotFound));
            }

            IAttemptResult<string> fileContentResult = this._fileIO.Read(targetFile);
            if (!fileContentResult.IsSucceeded || fileContentResult.Data is null)
            {
                return AttemptResult<CXml::PacketElement>.Fail(fileContentResult.Message);
            }

            CXml::PacketElement? comment;

            try
            {
                comment = Xmlparser.Deserialize<CXml::PacketElement>(fileContentResult.Data);
            }
            catch (Exception ex)
            {
                return AttemptResult<CXml::PacketElement>.Fail(this._errorHandler.HandleError(CommentRetreiverError.FailedToDeserializeComment, ex));
            }

            if (comment is null)
            {
                return AttemptResult<CXml::PacketElement>.Fail(this._errorHandler.HandleError(CommentRetreiverError.FailedToDeserializeComment));
            }

            return AttemptResult<CXml::PacketElement>.Succeeded(comment);
        }
    }
}
