using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Response = Niconicome.Models.Domain.Niconico.Net.Json.API.Comment.V3.Response;

namespace Niconicome.Models.Domain.Niconico.Download.Comment.V2.Core.Converter
{
    public interface INetCommentConverter
    {
        /// <summary>
        /// ネットワークのコメントを抽象コメントに変換
        /// </summary>
        /// <param name="chat"></param>
        /// <returns></returns>
        IComment ConvertNetCommentToCoreComment(Response::Comment chat, string thread, string fork);
    }

    public class NetCommentConverter : INetCommentConverter
    {
        public IComment ConvertNetCommentToCoreComment(Response::Comment chat, string thread, string fork)
        {
            var comment = new Comment()
            {
                Thread = thread,
                Fork = fork,
                Content = chat.Body,
                Command = chat.Commands,
                UserID = chat.UserId,
                No = chat.No,
                Vpos = chat.VposMs,
                Date = chat.PostedAt,
                Nicoru = chat.NicoruCount,
                IsPremium = chat.IsPremium,
                Score = chat.Score,
            };

            return comment;

        }

    }
}
