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
        IComment ConvertNetCommentToCoreComment(Chat chat);
    }

    public class NetCommentConverter
    {
    }
}
