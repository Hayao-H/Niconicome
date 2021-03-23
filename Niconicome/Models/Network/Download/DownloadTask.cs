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
        string NiconicoID { get; }
        string Title { get; }
        int PlaylistID { get; }
        int VideoID { get; }
        bool IsCanceled { get; set; }
        bool IsProcessing { get; set; }
        bool IsDone { get; set; }
        uint VerticalResolution { get; }
        DownloadSettings DownloadSettings { get; }
        CancellationToken CancellationToken { get; }
        event EventHandler<DownloadTaskMessageChangedEventArgs>? MessageChange;
        event EventHandler? ProcessingEnd;
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
        public DownloadTask(string niconicpoID, string title, int videoID, DownloadSettings downloadSettings)
        {
            this.DirectoryPath = downloadSettings.FolderPath;
            this.VerticalResolution = downloadSettings.VerticalResolution;
            this.messageFIeld = string.Empty;
            this.ID = Guid.NewGuid();
            this.PlaylistID = downloadSettings.PlaylistID;
            this.DownloadSettings = downloadSettings;
            this.NiconicoID = niconicpoID;
            this.VideoID = videoID;
            this.Title = title;
            this.cts = new CancellationTokenSource();
            this.CancellationToken = this.cts.Token;
        }

        protected string messageFIeld;

        protected readonly CancellationTokenSource cts;

        protected bool isProcessingField;

        /// <summary>
        /// メッセージ変更イベントを発火させる
        /// </summary>
        protected void RaiseMessageChanged()
        {
            this.MessageChange?.Invoke(this, new DownloadTaskMessageChangedEventArgs() { Message = this.Message });
        }

        /// <summary>
        /// 終了イベントを発火させる
        /// </summary>
        public void RaiseProcessingEnd()
        {
            this.ProcessingEnd?.Invoke(this, EventArgs.Empty);
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
        /// ニコニコ動画のID
        /// </summary>
        public string NiconicoID { get; init; }

        /// <summary>
        /// タイトル
        /// </summary>
        public string Title { get; init; }

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
        /// プレイリストID
        /// </summary>
        public int PlaylistID { get; init; }

        /// <summary>
        /// 動画ID
        /// </summary>
        public int VideoID { get; init; }

        /// <summary>
        /// キャンセルフラグ
        /// </summary>
        public virtual bool IsCanceled { get; set; }

        /// <summary>
        /// 実行中フラグ
        /// </summary>
        public virtual bool IsProcessing
        {
            get => this.isProcessingField;
            set
            {
                this.isProcessingField = value;
                if (!value)
                {
                    this.RaiseProcessingEnd();
                }
            }
        }

        /// <summary>
        /// 完了フラグ
        /// </summary>
        public virtual bool IsDone { get; set; }

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
        /// 終了イベント
        /// </summary>
        public event EventHandler? ProcessingEnd;

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
        public BindableDownloadTask(string niconicpoID, string title, int videoID, DownloadSettings downloadSettings) : base(niconicpoID, title, videoID, downloadSettings)
        {
        }

        private bool isCanceledField;

        private bool isDoneField;

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

        public override bool IsProcessing
        {
            get => this.isProcessingField;
            set
            {
                this.SetProperty(ref this.isProcessingField, value);
                if (!value)
                {
                    this.RaiseProcessingEnd();
                }
            }
        }

        public override bool IsDone { get => this.isDoneField; set => this.SetProperty(ref this.isDoneField, value); }
    }
}
