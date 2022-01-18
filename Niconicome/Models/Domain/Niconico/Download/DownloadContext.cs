using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niconicome.Models.Domain.Niconico.Download
{

    public interface IDownloadContext
    {
        /// <summary>
        /// コンテクストID
        /// </summary>
        string ContextID { get; }

        /// <summary>
        /// ニコニコのID
        /// </summary>
        string NiconicoId { get; }

        /// <summary>
        /// 動画ファイル名
        /// </summary>
        string FileName { get; set; }

        /// <summary>
        /// 実際の解像度
        /// </summary>
        uint ActualVerticalResolution { get; set; }

        /// <summary>
        /// 動画のセグメント数
        /// </summary>
        int OriginalSegmentsCount { get; set; }

        /// <summary>
        /// ログ出力
        /// </summary>
        /// <returns></returns>

        string GetLogContent();
    }

    public class DownloadContext : IDownloadContext
    {
        public DownloadContext(string niconicoId)
        {
            this.NiconicoId = niconicoId;
            this.ContextID = Guid.NewGuid().ToString("D");
        }

        public string ContextID { get; init; }

        public string NiconicoId { get; init; }

        public uint ActualVerticalResolution { get; set; }

        public string FileName { get; set; } = string.Empty;

        public int OriginalSegmentsCount { get; set; }

        public string GetLogContent()
        {
            return $"context_id: {this.ContextID}, content_id: {this.NiconicoId}";
        }

    }
}
