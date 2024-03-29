﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niconicome.Models.Domain.Niconico.Download.Video.V2.Local.HLS
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
        IReadOnlyCollection<string> ExistingFileNames { get; }
    }

    public record SegmentDirectoryInfo(string DirectoryPath, DateTime DownloadStartedOn, IReadOnlyCollection<string> ExistingFileNames) : ISegmentDirectoryInfo;

}
