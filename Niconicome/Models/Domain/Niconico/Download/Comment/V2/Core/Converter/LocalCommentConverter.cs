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
                No = chat.No,
                Vpos = chat.Vpos * 10,
                Date = DateTimeOffset.FromUnixTimeSeconds(chat.Date).ToLocalTime().DateTime,
                UserID = chat.UserId,
                Command = chat.Mail?.Split(' ').ToList() ?? new List<string>(),
                Content = chat.Text,
                Score = chat.Score,
            };

            return comment;
        }

        public V2::ChatElement ConvertCoreCommentToChat(IComment comment)
        {
            var chat = new V2::ChatElement()
            {
                Text = comment.Content ?? string.Empty,
                Mail = string.Join(' ', comment.Command),
                UserId = comment.UserID ?? string.Empty,
                Thread = comment.Thread,
                No = comment.No,
                Vpos = comment.Vpos / 10,
                Score = comment.Score,
                Date = new DateTimeOffset(DateTime.SpecifyKind(comment.Date, DateTimeKind.Unspecified), TimeSpan.FromHours(9)).ToUnixTimeSeconds(),
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
