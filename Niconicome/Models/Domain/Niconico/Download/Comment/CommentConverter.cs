using System;
using System.Collections.Generic;
using System.Linq;
using Xml = Niconicome.Models.Domain.Niconico.Net.Xml.Comment;
using System.Text;
using System.Threading.Tasks;
using Response = Niconicome.Models.Domain.Niconico.Net.Json.API.Comment.Response;
using Utils = Niconicome.Models.Domain.Utils;

namespace Niconicome.Models.Domain.Niconico.Download.Comment
{

    public interface IStoreCommentsData
    {
        Xml::Packet Chats { get; }
        Xml::Packet? OwnerComments { get; }
        string Filename { get; set; }
        string OwnerFilename { get; set; }
    }

    public class StoreCommentsData : IStoreCommentsData
    {
        public Xml::Packet Chats { get; set; } = new();

        public Xml::Packet? OwnerComments { get; set; }

        public string Filename { get; set; } = string.Empty;

        public string OwnerFilename { get; set; } = string.Empty;

    }

    interface ICommentConverter
    {
        IStoreCommentsData ConvertToStoreCommentsData(ICommentCollection comments, ICommentDownloadSettings settings);
    }

    /// <summary>
    /// コメントの形式を変換する
    /// </summary>
    class CommentConverter : ICommentConverter
    {
        /// <summary>
        /// XML形式のデータに変換する
        /// </summary>
        /// <param name="comments"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public IStoreCommentsData ConvertToStoreCommentsData(ICommentCollection comments, ICommentDownloadSettings settings)
        {
            var copy = comments.Clone();
            this.RemoveOwnerComment(comments);

            var data = new StoreCommentsData()
            {
                Chats = this.ConvertToXmlData(comments)
            };

            if (settings.IsDownloadingLogEnable)
            {
                this.GetOwnerComment(copy);
                if (copy.Count > 0)
                {
                    data.OwnerComments = this.ConvertToXmlData(copy);
                }
            }

            return data;
        }

        /// <summary>
        /// 投コメを取得する
        /// </summary>
        /// <param name="comments"></param>
        /// <returns></returns>
        private void GetOwnerComment(ICommentCollection comments)
        {
            comments.Where(c => c.Chat is not null && this.IsOwnerComment(c.Chat));
        }

        /// <summary>
        /// 投コメをフィルターする
        /// </summary>
        /// <param name="comments"></param>
        /// <returns></returns>
        private void RemoveOwnerComment(ICommentCollection comments)
        {
            comments.Where(c => c.Chat is not null && !this.IsOwnerComment(c.Chat));
        }

        /// <summary>
        /// 投コメの判断
        /// </summary>
        /// <param name="comment"></param>
        /// <returns></returns>
        private bool IsOwnerComment(Response::Chat comment)
        {
            return comment.UserId is null && comment.Deleted is null;
        }

        /// <summary>
        /// XMLの形式に変換する
        /// </summary>
        /// <param name="comments"></param>
        /// <returns></returns>
        private Xml::Packet ConvertToXmlData(ICommentCollection comments)
        {
            return new Xml::Packet()
            {
                Chat = comments.GetAllComments().Select(c =>
                 new Xml::PacketChat()
                 {
                     Thread = c.Thread ?? string.Empty,
                     No = c.No,
                     Vpos = c.Vpos,
                     Date = c.Date,
                     Anonymity = c.Anonymity ?? 0,
                     UserId = c.UserId,
                     Mail = c.Mail,
                     Text = Utils::XmlUtils.RemoveSymbols(c.Content),
                     Premium = (int)(c.Premium ?? 1),
                 }
            ).ToList(),
                Thread = new Xml::PacketThread()
                {
                    Resultcode = comments.ThreadInfo?.Resultcode ?? 0,
                    Thread = comments.ThreadInfo?.ThreadThread ?? string.Empty,
                    ServerTime = comments.ThreadInfo?.ServerTime ?? 0,
                    LastRes = comments.ThreadInfo?.LastRes ?? 0,
                    Ticket = comments.ThreadInfo?.Ticket,
                    Revision = comments.ThreadInfo?.Revision ?? 0
                },
            };
        }


    }
}
