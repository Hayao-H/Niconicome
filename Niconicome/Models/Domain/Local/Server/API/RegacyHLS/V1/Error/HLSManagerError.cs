using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Utils.Error;

namespace Niconicome.Models.Domain.Local.Server.API.RegacyHLS.V1.Error
{
    public enum HLSManagerError
    {
        [ErrorEnum(ErrorLevel.Error, "動画がダウンロードされていません。(id:{0}, playlist:{1})")]
        VideoIsNotDownloaded,
        [ErrorEnum(ErrorLevel.Error, "動画ファイルが存在しません。(path:{0})")]
        FileDoesNotExist,
        [ErrorEnum(ErrorLevel.Error, "ファイルのHLS化に失敗しました。")]
        FailedToEncodeFileToHLS,
        [ErrorEnum(ErrorLevel.Error, "プレイリストと動画IDを抽出できませんでした。")]
        CannotExtractPlaylistAndVideoID,
        [ErrorEnum(ErrorLevel.Error, "新サーバーの動画です。(id:{0}, playlist:{1})")]
        VideoIsDMS,
        [ErrorEnum(ErrorLevel.Error, "現在変換を実行中です。")]
        AlreadyRunning,
    }
}
