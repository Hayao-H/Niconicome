using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CXml = Niconicome.Models.Domain.Niconico.Net.Xml.Comment.V2;


namespace Niconicome.Models.Domain.Local.Server.API.Comment.V1.Local
{
    public interface ICommentConverter
    {
        CommentType Convert(CXml::PacketElement packet);
    }

    public class CommentConverter : ICommentConverter
    {
        public CommentType Convert(CXml::PacketElement packet)
        {
            CommentType commentType = new CommentType();
            foreach (var chat in packet.Chat)
            {
                Comment comment = new Comment()
                {
                    Body = chat.Text,
                    UserID = chat.UserId,
                    VposMS = chat.Vpos * 10,
                    Mail = chat.Mail,
                    PostedAt = DateTimeOffset.FromUnixTimeSeconds(chat.Date).ToLocalTime().DateTime,
                    Number = chat.No,
                };
                commentType.Comments.Add(comment);
            }
            return commentType;
        }
    }
}
