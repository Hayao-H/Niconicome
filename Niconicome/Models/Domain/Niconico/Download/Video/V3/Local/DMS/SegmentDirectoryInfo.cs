using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niconicome.Models.Domain.Niconico.Download.Video.V3.Local.DMS
{
    public interface ISegmentDirectoryInfo
    {
        /// <summary>
        /// ディレクトリパス
        /// </summary>
        string DirectoryPath { get; }

        /// <summary>
        /// DL開始日時
        /// </summary>
        DateTime DownloadStartedOn { get; }

        /// <summary>
        /// 存在するセグメントファイル名
        /// </summary>
        IReadOnlyCollection<string> ExistingVideoFileNames { get; }

        /// <summary>
        /// 存在する音声ファイル名
        /// </summary>
        IReadOnlyCollection<string> ExistingAudioFileNames { get; }
    }

    public record SegmentDirectoryInfo(string DirectoryPath, DateTime DownloadStartedOn, IReadOnlyCollection<string> ExistingVideoFileNames, IReadOnlyCollection<string> ExistingAudioFileNames) : ISegmentDirectoryInfo;
}
