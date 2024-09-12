using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Utils.Error;

namespace Niconicome.Models.Domain.Niconico.Download.Video.V2.Error
{
    public enum WatchSessionError
    {
        [ErrorEnum(ErrorLevel.Error,"セッションが失効しています。")]
        SessionExpired,
        [ErrorEnum(ErrorLevel.Error,"セッションが確立されていません。")]
        SessionNotEnsured,
        [ErrorEnum(ErrorLevel.Error,"暗号化された動画のため、ダウンロードできません。(id:{0})")]
        VideoIsEncrypted,
        [ErrorEnum(ErrorLevel.Error, "有料動画のため、ダウンロードできません。(id:{0})")]
        VideoRequirePayment,
        [ErrorEnum(ErrorLevel.Error, "セッションの確立に失敗しました。(id:{0})")]
        SessionEnsuringFailure,
        [ErrorEnum(ErrorLevel.Log, "{0}の視聴セッションを確立しました。")]
        SessionEnsured,
        [ErrorEnum(ErrorLevel.Error, "ハートビートの送信に失敗しました。(session_id:{0},status: {0})")]
        FailedToSendHeartBeat,
        [ErrorEnum(ErrorLevel.Log, "ハートビートの送信に成功しました。(session_id:{0})")]
        SucceededToSendHeartBeat,
        [ErrorEnum(ErrorLevel.Error,"アドオンが不正な情報を返却しました。(id:{0})")]
        AddonReturnedInvalidInfomation,
        [ErrorEnum(ErrorLevel.Error, "セッション確立アドオンが登録されていません。")]
        AddonNotRegistered,
    }
}
