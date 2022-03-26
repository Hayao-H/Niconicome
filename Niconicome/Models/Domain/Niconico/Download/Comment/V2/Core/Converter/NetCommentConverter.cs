using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Niconico.Net.Json.API.Comment.Response;

namespace Niconicome.Models.Domain.Niconico.Download.Comment.V2.Core.Converter
{
    public interface INetCommentConverter
    {
        Comment ConvertNetCommentToCoreComment(Chat chat);
    }

    internal class NetCommentConverter
    {
    }
}
