using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using V2 = Niconicome.Models.Domain.Niconico.Net.Xml.Comment.V2;

namespace Niconicome.Models.Domain.Niconico.Download.Comment.V2.Core.Converter
{
    public interface ILocalCommentConverter
    {
        /// <summary>
        /// ローカルのコメントを抽象コメントに変換
        /// </summary>
        /// <param name="chat"></param>
        /// <returns></returns>
        IComment ConvertChatToCoreComment(V2::ChatElement chat);

        /// <summary>
        /// 抽象コメントをローカルのコメントに変換
        /// </summary>
        /// <param name="comment"></param>
        /// <returns></returns>
        V2::ChatElement ConvertCoreCommentToChat(IComment comment);

        /// <summary>
        /// 抽象スレッド情報をローカルのスレッド情報に変換
        /// </summary>
        /// <param name="threadInfo"></param>
        /// <returns></returns>
        V2::ThreadElement ConvertCoreThreadInfoToThread(IThreadInfo threadInfo);
    }

    public class LocalCommentConverter : ILocalCommentConverter
    {
        public IComment ConvertChatToCoreComment(V2::ChatElement chat)
        {
            var comment = new Comment()
            {
                Thread = chat.Thread,
                Fork = chat.Fork,
                No = chat.No,
                Vpos = chat.Vpos,
                Date = chat.Date,
                DateUsec = chat.DateUsec,
                Anonimity = chat.Anonymity,
                UserID = chat.UserId,
                Mail = chat.Mail,
                Content = chat.Text,
                Premium = chat.Premium,
                Score = chat.Score,
                Nicoru = chat.Nicoru,
            };

            return comment;
        }

        public V2::ChatElement ConvertCoreCommentToChat(IComment comment)
        {
            var chat = new V2::ChatElement()
            {
                Text = comment.Content,
                Mail = comment.Mail,
                UserId = comment.UserID,
                Thread = comment.Thread,
                No = comment.No,
                Vpos = comment.Vpos,
                Premium = comment.Premium ?? 0,
                Anonymity = comment.Anonimity,
                Nicoru = comment.Nicoru,
                Deleted = comment.Deleted ?? 0,
                Score = comment.Score,
                Date = comment.Date,
                DateUsec = comment.DateUsec,
            };

            return chat;
        }

        public V2::ThreadElement ConvertCoreThreadInfoToThread(IThreadInfo threadInfo)
        {
            var t = new V2::ThreadElement()
            {
                Resultcode = threadInfo.ResultCode,
                Thread = threadInfo.Thread,
                Ticket = threadInfo.Ticket,
                ServerTime = threadInfo.ServerTime,
                LastRes = threadInfo.LastRes,
                Revision = threadInfo.Revision,
            };

            return t;
        }
    }
}
