using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Const;

namespace Niconicome.Models.Domain.Niconico.Download.Video.V2.Local.Encode
{
    public interface IEncodeOption
    {
        /// <summary>
        /// エンコード後の保存ファイルパス
        /// </summary>
        string FilePath { get; }

        /// <summary>
        /// セグメントファイルのフォルダーパス
        /// </summary>
        string FolderPath { get; }

        /// <summary>
        /// コマンドフォーマット
        /// </summary>
        string CommandFormat { get; }

        /// <summary>
        /// 日付情報を上書き
        /// </summary>
        bool IsOverrideDTEnable { get; }

        /// <summary>
        /// 結合した.tsファイルをそのままコピー
        /// </summary>
        bool IsStoreRawTSFileEnable { get; }

        /// <summary>
        /// 投稿日時
        /// </summary>
        DateTime UploadedOn { get; }

        /// <summary>
        /// セグメントファイルのパス
        /// </summary>
        IEnumerable<string> TsFilePaths { get; }
    }

    public class EncodeOption : IEncodeOption
    {
        public string FilePath { get; init; } = string.Empty;

        public string CommandFormat { get; init; } = Format.DefaultFFmpegFormat;

        public string FolderPath { get; init; } = string.Empty;


        public bool IsOverwriteEnable { get; init; }

        public bool IsOverrideDTEnable { get; init; }

        public bool IsStoreRawTSFileEnable { get; init; }

        public DateTime UploadedOn { get; init; }

        public IEnumerable<string> TsFilePaths { get; init; } = new List<string>();
    }
}
