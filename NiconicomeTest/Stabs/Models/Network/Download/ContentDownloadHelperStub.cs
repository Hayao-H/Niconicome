using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Niconico.Download;
using Niconicome.Models.Helper.Result;
using Niconicome.Models.Network.Download;
using Niconicome.Models.Playlist;

namespace NiconicomeTest.Stabs.Models.Network.Download
{
    internal class ContentDownloadHelperStub : IContentDownloadHelper
    {
        public Task<IAttemptResult<IDownloadContext>> TryDownloadContentAsync(IListVideoInfo videoInfo, IDownloadSettings setting, Action<string> OnMessage, CancellationToken token)
        {
            return Task.FromResult<IAttemptResult<IDownloadContext>>(AttemptResult<IDownloadContext>.Succeeded(new DownloadContext(string.Empty)));
        }

    }
}
