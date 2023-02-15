using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Utils.Error;

namespace Niconicome.Models.Local.State.MessageV2
{
    public record Message(string Content, string Dispacer, DateTime AddedAt, ErrorLevel ErrorLevel);
}
