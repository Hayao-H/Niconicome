using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niconicome.Models.Domain.Niconico.Download
{

    public interface IDownloadContext
    {
        string NiconicoId { get; }
        string Id { get; }
        uint ActualVerticalResolution { get; set; }

        string GetLogContent();
    }

    public class DownloadContext : IDownloadContext
    {
        public DownloadContext(string niconicoId)
        {
            this.NiconicoId = niconicoId;
            this.Id = Guid.NewGuid().ToString("D");
        }

        /// <summary>
        /// 動画ID
        /// </summary>
        public string NiconicoId { get; init; }

        /// <summary>
        /// ユニークなID
        /// </summary>
        public string Id { get; init; }

        /// <summary>
        /// 実際の垂直解像度
        /// </summary>
        public uint ActualVerticalResolution { get; set; }


        /// <summary>
        /// ログ出力可能な文字列を取得する
        /// </summary>
        /// <returns></returns>
        public string GetLogContent()
        {
            return $"context_id: {this.Id}, content_id: {this.NiconicoId}";
        }

    }
}
