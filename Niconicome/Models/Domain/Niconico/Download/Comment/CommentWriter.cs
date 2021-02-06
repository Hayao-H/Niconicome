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
        void Write(IStoreCommentsData comments, string folderPath, bool overwrite);
    }

    /// <summary>
    /// コメントをローカルに保存する
    /// </summary>
    public class CommentStream : ICommentStream
    {

        /// <summary>
        /// コメントを保存する
        /// </summary>
        /// <param name="comments"></param>
        /// <param name="folderPath"></param>
        public void Write(IStoreCommentsData comments, string folderPath, bool overwrite)
        {
            string targetPath = this.GetFilePath(folderPath, comments.Filename, overwrite);
            this.WriteComments(comments.Chats, targetPath);

            if (comments.OwnerComments is not null)
            {
                string ownerFIlePath = this.GetFilePath(folderPath, comments.OwnerFilename, overwrite);
                this.WriteComments(comments.OwnerComments, ownerFIlePath);
            }
        }

        private string GetFilePath(string folderPath,string fileName,bool overwrite)
        {
            var path = Path.Combine(folderPath, fileName);
            if (!overwrite)
            {
                path = this.GetModifiedFilePath(path);
            }

            return path;
        }

        private string GetModifiedFilePath(string old)
        {
            return IOUtils.CheclFileExistsAndReturnNewFilename(old);
        }

        /// <summary>
        /// コメントを書き込む
        /// </summary>
        /// <param name="comments"></param>
        /// <param name="folderPath"></param>
        /// <param name="filename"></param>
        private void WriteComments(Xml::Packet comments, string targetPath)
        {
            string content = Xmlparser.Serialize(comments);

            using var fs = new StreamWriter(targetPath);
            fs.Write(content);
        }
    }


}
