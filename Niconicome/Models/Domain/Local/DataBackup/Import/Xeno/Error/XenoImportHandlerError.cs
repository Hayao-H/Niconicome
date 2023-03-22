using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Utils.Error;
using Niconicome.Models.Domain.Utils.StringHandler;

namespace Niconicome.Models.Domain.Local.DataBackup.Import.Xeno.Error
{
    public enum XenoImportHandlerError
    {
        [ErrorEnum(ErrorLevel.Warning,"プレイリストのインポートに失敗しました。(name:{0})")]
        FailedToImportPlaylist,
        [ErrorEnum(ErrorLevel.Warning, "動画のインポートに失敗しました。(playlistName:{0}, id:{1})")]
        FailedToImportVideo,
    }
}
