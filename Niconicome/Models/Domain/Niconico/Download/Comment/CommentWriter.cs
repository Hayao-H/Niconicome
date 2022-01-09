using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Niconico.Net.Xml;
using Niconicome.Models.Domain.Utils;
using Xml = Niconicome.Models.Domain.Niconico.Net.Xml.Comment;

namespace Niconicome.Models.Domain.Niconico.Download.Comment
{
    public interface ICommentStream
    {
        /// <summary>
        /// コメントを書き込む
        /// </summary>
        /// <param name="comments"></param>
        /// <param name="overwrite"></param>
        void Write(IStoreCommentsData comments, bool overwrite);
    }

    /// <summary>
    /// コメントをローカルに保存する
    /// </summary>
    public class CommentStream : ICommentStream
    {

        #region Method

        public void Write(IStoreCommentsData comments, bool overwrite)
        {
            string targetPath = comments.FilePath;
            this.WriteComments(comments.Chats, targetPath);

            if (comments.OwnerComments is not null)
            {
                string ownerFIlePath = comments.OwnerFilPath;
                this.WriteComments(comments.OwnerComments, ownerFIlePath);
            }
        }

        #endregion

        #region private

        /// <summary>
        /// コメントを書き込む
        /// </summary>
        private void WriteComments(Xml::Packet comments, string targetPath)
        {
            string content = Xmlparser.Serialize(comments);

            IOUtils.CreateDirectoryIfNotExist(targetPath);

            using var fs = new StreamWriter(targetPath);
            fs.Write(content);
        }

        #endregion
    }


}
