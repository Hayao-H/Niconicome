using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Niconico.Net.Json.API.Comment.V2.Response;

namespace Niconicome.Models.Domain.Niconico.Download.Comment.V2.Core.Converter
{
    public interface INetCommentConverter
    {
        /// <summary>
        /// ネットワークのコメントを抽象コメントに変換
        /// </summary>
        /// <param name="chat"></param>
        /// <returns></returns>
        IComment ConvertNetCommentToCoreComment(Chat chat);

        /// <summary>
        /// ネットワークのスレッド情報を抽象スレッド情報に変換
        /// </summary>
        /// <param name="thread"></param>
        /// <returns></returns>
        IThreadInfo ConvertNetThreadToCoreThreadInfo(Thread thread);
    }

    public class NetCommentConverter : INetCommentConverter
    {
        public IComment ConvertNetCommentToCoreComment(Chat chat)
        {
            var comment = new Comment()
            {
                Thread = chat.Thread,
                Fork = chat.Fork ?? 0,
                Content = chat.Content,
                Mail = chat.Mail,
                UserID = chat.UserID,
                No = chat.No,
                Vpos = chat.Vpos,
                Date = chat.Date,
                DateUsec = chat.DateUsec,
                Nicoru = chat.Nicoru,
                Premium = chat.Premium,
                Anonimity = chat.Anonymity,
                Score = chat.Score,
                Deleted = chat.Deleted,
            };

            return comment;

        }

        public IThreadInfo ConvertNetThreadToCoreThreadInfo(Thread thread)
        {
            var threadInfo = new ThreadInfo()
            {
                ResultCode = thread.ResultCode,
                Thread = thread.ThreadID,
                ServerTime = thread.ServerTime,
                LastRes = thread.LastRes,
                Ticket = thread.Ticket,
                Revision = thread.Revision,
            };

            return threadInfo;
        }
    }
}
