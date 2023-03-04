using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Utils.StringHandler;

namespace Niconicome.Models.Domain.Local.Server.RequestHandler.NotFound
{
    public enum NotFoundRequestHandlerStringContent
    {
        [StringEnum(@"<!DOCTYPE html><html><head><meta charset=""utf-8""/><title>404 | Niconicome</title></head><body>要求されたリソースが見つかりませんでした。(url:{0})</body></html>")]
        NotFound,
        [StringEnum(@"<!DOCTYPE html><html><head><meta charset=""utf-8""/><title>404 | Niconicome</title></head><body>{0}</body></html>")]
        ErrorOccured,
    }
}
