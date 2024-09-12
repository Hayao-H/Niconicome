using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Utils.Error;

namespace Niconicome.Models.Network.Download.DLTask.Error
{
    public enum DownloadManagerError
    {
        [ErrorEnum(ErrorLevel.Error, "ダウンロード中にエラーが発生しました")]
        Error,
    }
}
