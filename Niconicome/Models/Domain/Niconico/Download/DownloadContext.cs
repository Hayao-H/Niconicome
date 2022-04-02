using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.I2c.Provider;

namespace Niconicome.Models.Domain.Niconico.Download
{

    public interface IDownloadContext : IDisposable
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
        /// コメント数
        /// </summary>
        int CommentCount { get; set; }

        /// <summary>
        /// ログ出力
        /// </summary>
        /// <returns></returns>
        string GetLogContent();

        /// <summary>
        /// メッセージをハンドラに送信する
        /// </summary>
        /// <param name="content"></param>
        void SendMessage(string content);

        /// <summary>
        /// メッセージハンドラを追加
        /// </summary>
        /// <param name="handler"></param>
        void RegisterMessageHandler(Action<string> handler);
    }

    public class DownloadContext : IDownloadContext
    {
        public DownloadContext(string niconicoId)
        {
            this.NiconicoId = niconicoId;
            this.ContextID = Guid.NewGuid().ToString("D");
        }

        #region field

        private readonly List<Action<string>> messageHandlers = new();

        #endregion

        #region Props

        public string ContextID { get; init; }

        public string NiconicoId { get; init; }

        public uint ActualVerticalResolution { get; set; }

        public string FileName { get; set; } = string.Empty;

        public int OriginalSegmentsCount { get; set; }

        public int CommentCount { get; set; }

        #endregion

        #region Method

        public string GetLogContent()
        {
            return $"context_id: {this.ContextID}, content_id: {this.NiconicoId}";
        }

        public void RegisterMessageHandler(Action<string> handler)
        {
            this.messageHandlers.Add(handler);
        }

        public void SendMessage(string content)
        {
            foreach (var handler in this.messageHandlers)
            {
                try
                {
                    handler(content);
                }
                catch { }
            }
        }

        public void Dispose()
        {
            this.messageHandlers.Clear();
        }

        #endregion

    }
}
