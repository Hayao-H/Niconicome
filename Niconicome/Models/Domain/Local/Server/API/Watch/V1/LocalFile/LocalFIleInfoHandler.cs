using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Helper.Result;

namespace Niconicome.Models.Domain.Local.Server.API.Watch.V1.LocalFile
{
    public interface ILocalFileInfoHandler
    {
        IAttemptResult<ILocalFileInfo> GetLocalFileInfo(string filePath);
    }
}
