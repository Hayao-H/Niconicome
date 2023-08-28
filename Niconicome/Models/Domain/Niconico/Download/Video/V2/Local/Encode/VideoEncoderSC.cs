using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Utils.StringHandler;

namespace Niconicome.Models.Domain.Niconico.Download.Video.V2.Local.Encode
{
    public enum VideoEncoderSC
    {
        [StringEnum("セグメントファイルの結合を開始")]
        StartTSConcat,
        [StringEnum("セグメントファイルの結合が完了")]
        CompleteTSConcat,
        [StringEnum("動画ファイルをコピー中")]
        StartCopyFile,
        [StringEnum("動画ファイルのコピーが完了")]
        CompleteCopyFile,
        [StringEnum("ffmpegで変換を開始(.ts=>.mp4)")]
        StartEncode,
        [StringEnum("ffmpegの変換が完了")]
        CompleteEncode,
    }
}
