using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Utils.Error;

namespace Niconicome.Models.Playlist.V2.Utils.Error
{
    public enum VideoInfoCopyManagerError
    {
        [ErrorEnum(ErrorLevel.Error, "コピー元の動画が空です。")]
        SourceIsEmpty,
    }
}
