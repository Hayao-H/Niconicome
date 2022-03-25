using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Niconico.Download.Comment.V2.Fetch;
using Niconicome.Models.Helper.Result;

namespace NiconicomeTest.Stabs.Models.Domain.Niconico.Download.Comment.V2.Fetch
{
    public class OfficialCommentHandlerStub:IOfficialCommentHandler
    {
        public Task<IAttemptResult<WayBackKey>> GetWayBackKeyAsync(string thread)
        {
            return Task.FromResult(AttemptResult<WayBackKey>.Succeeded(new WayBackKey(Key)));
        }

        public static string Key => "2525.testkey";
    }
}
