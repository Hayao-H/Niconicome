using System;
using System.Threading;
using Niconicome.Models.Playlist;
using Niconicome.ViewModels;
using Reactive.Bindings;

namespace Niconicome.Models.Network.Download
{

    public interface IDownloadTask
    {
        Guid ID { get; }
        string DirectoryPath { get; }
        ReactiveProperty<string> Message { get;  }
        string NiconicoID { get; }
        string Title { get; }
        int PlaylistID { get; }
        int VideoID { get; }
        ReactiveProperty<bool> IsCanceled { get; }
        ReactiveProperty<bool> IsProcessing { get; }
        ReactiveProperty<bool> IsDone { get; }
        uint VerticalResolution { get; }
        DownloadSettings DownloadSettings { get; }
        CancellationToken CancellationToken { get; }
        void Cancel();
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
            this.Message = new ReactiveProperty<string>();
            this.IsCanceled = new ReactiveProperty<bool>();
            this.IsProcessing = new ReactiveProperty<bool>();
            this.IsDone = new ReactiveProperty<bool>();
            this.ID = Guid.NewGuid();
            this.PlaylistID = downloadSettings.PlaylistID;
            this.DownloadSettings = downloadSettings;
            this.NiconicoID = niconicpoID;
            this.VideoID = videoID;
            this.Title = title;
            this.cts = new CancellationTokenSource();
            this.CancellationToken = this.cts.Token;
        }


        protected readonly CancellationTokenSource cts;

        protected bool isProcessingField;

        protected bool isDoneField;

        /// <summary>
        /// 終了イベントを発火させる
        /// </summary>
        protected void RaiseProcessingEnd()
        {
            this.ProcessingEnd?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// 開始イベントを発火させる
        /// </summary>
        protected void RaiseProcessingStart()
        {
            this.ProcessStart?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// 終了イベントを発火させる
        /// </summary>
        protected void RaiseDone()
        {
            this.Done?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// キャンセルイベントを発火させる
        /// </summary>
        protected void RaiseCancel()
        {
            this.TaskCancel?.Invoke(this, EventArgs.Empty);
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
        public ReactiveProperty<string> Message { get; init; }

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
        public ReactiveProperty<bool> IsCanceled { get; init; }

        /// <summary>
        /// 実行中フラグ
        /// </summary>
        public ReactiveProperty<bool> IsProcessing { get; init; }

        /// <summary>
        /// 完了フラグ
        /// </summary>
        public ReactiveProperty<bool> IsDone { get; init; }

        /// <summary>
        /// /垂直解像度
        /// </summary>
        public uint VerticalResolution { get; init; }

        /// <summary>
        /// ダウンロード設定
        /// </summary>
        public DownloadSettings DownloadSettings { get; init; }

        /// <summary>
        /// 終了イベント
        /// </summary>
        public event EventHandler? ProcessingEnd;

        /// <summary>
        /// 開始イベント
        /// </summary>
        public event EventHandler? ProcessStart;

        /// <summary>
        /// 完了イベント
        /// </summary>
        public event EventHandler? Done;

        /// <summary>
        /// キャンセルイベント
        /// </summary>
        public event EventHandler? TaskCancel;

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
            this.IsCanceled.Value = true;
            this.Message.Value = "DLをキャンセル";
            this.IsProcessing.Value = false;
            this.RaiseCancel();
        }

    }
}
