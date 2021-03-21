using System;
using System.Threading;
using Niconicome.Models.Playlist;
using Niconicome.ViewModels;

namespace Niconicome.Models.Network.Download
{

    public interface IDownloadTask
    {
        Guid ID { get; }
        string DirectoryPath { get; }
        string Message { get; set; }
        int PlaylistID { get; }
        bool IsCanceled { get; set; }
        bool IsProcessing { get; set; }
        uint VerticalResolution { get; }
        DownloadSettings DownloadSettings { get; }
        IVideoListInfo Video { get; }
        CancellationToken CancellationToken { get; }
        event EventHandler<DownloadTaskMessageChangedEventArgs>? MessageChange;
        void Cancel();
    }

    public class DownloadTaskMessageChangedEventArgs : EventArgs
    {
        public string? Message { get; set; }
    }

    /// <summary>
    /// ダウンロードタスク
    /// </summary>
    public record DownloadTask : BindableRecordBase, IDownloadTask
    {
        public DownloadTask(IVideoListInfo video, DownloadSettings downloadSettings)
        {
            this.DirectoryPath = downloadSettings.FolderPath;
            this.VerticalResolution = downloadSettings.VerticalResolution;
            this.messageFIeld = string.Empty;
            this.ID = Guid.NewGuid();
            this.PlaylistID = downloadSettings.PlaylistID;
            this.DownloadSettings = downloadSettings;
            this.Video = video;
            this.cts = new CancellationTokenSource();
            this.CancellationToken = this.cts.Token;
        }

        protected string messageFIeld;

        protected readonly CancellationTokenSource cts;

        /// <summary>
        /// メッセージ変更イベントを発火させる
        /// </summary>
        protected void RaiseMessageChanged()
        {
            this.MessageChange?.Invoke(this, new DownloadTaskMessageChangedEventArgs() { Message = this.Message });
        }

        /// <summary>
        /// ダウンロードタスクID
        /// </summary>
        public Guid ID { get; init; }

        /// <summary>
        /// 保存フォルダーパス
        /// </summary>
        public string DirectoryPath { get; init; }

        /// <summary>
        /// メッセージ
        /// </summary>
        public virtual string Message
        {
            get => this.messageFIeld;
            set
            {
                this.messageFIeld = value;
                this.RaiseMessageChanged();
            }
        }

        /// <summary>
        /// 動画情報
        /// </summary>
        public IVideoListInfo Video { get; init; }

        /// <summary>
        /// プレイリストID
        /// </summary>
        public int PlaylistID { get; init; }

        /// <summary>
        /// キャンセルフラグ
        /// </summary>
        public virtual bool IsCanceled { get; set; }

        /// <summary>
        /// 実行中フラグ
        /// </summary>
        public virtual bool IsProcessing { get; set; }

        /// <summary>
        /// /垂直解像度
        /// </summary>
        public uint VerticalResolution { get; init; }

        /// <summary>
        /// ダウンロード設定
        /// </summary>
        public DownloadSettings DownloadSettings { get; init; }


        /// <summary>
        /// メッセージ変更イベント
        /// </summary>
        public event EventHandler<DownloadTaskMessageChangedEventArgs>? MessageChange;

        /// <summary>
        /// トークン
        /// </summary>
        public CancellationToken CancellationToken { get; init; }

        /// <summary>
        /// タスクをキャンセル
        /// </summary>
        public void Cancel()
        {
            this.cts.Cancel();
            this.IsCanceled = true;
            this.IsProcessing = false;
        }

    }

    /// <summary>
    /// バインド可能なタスク
    /// </summary>
    public record BindableDownloadTask : DownloadTask
    {
        public BindableDownloadTask(IVideoListInfo video, DownloadSettings downloadSettings) : base(video, downloadSettings)
        {
        }

        private bool isProcessingField;

        private bool isCanceledField;

        public override string Message
        {
            get => this.messageFIeld;
            set
            {
                this.SetProperty(ref this.messageFIeld, value);
                this.RaiseMessageChanged();
            }
        }

        public override bool IsCanceled { get => this.isCanceledField; set => this.SetProperty(ref this.isCanceledField, value); }

        public override bool IsProcessing { get => this.isProcessingField; set => this.SetProperty(ref this.isProcessingField, value); }
    }
}
