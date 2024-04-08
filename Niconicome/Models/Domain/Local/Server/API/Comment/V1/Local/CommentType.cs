using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niconicome.Models.Domain.Local.Server.API.Comment.V1.Local
{
    public class CommentType
    {
        public List<Comment> Comments { get; init; } = new();
    }

    public class Comment
    {
        public required string Body { get; init; }

        public required string UserID { get; init; }

        public required int VposMS { get; init; }

        public required string Mail { get; init; }
    }
}
