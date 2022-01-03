using System;
using System.Threading;
using System.Threading.Tasks;
using Niconicome.Models.Playlist;
using Niconicome.Models.Utils;
using Niconicome.ViewModels;
using Reactive.Bindings;

namespace Niconicome.Models.Network.Download
{

    public interface IDownloadTask : IParallelTask<IDownloadTask>
    {
        /// <summary>
        /// タイトル
        /// </summary>
        string Title { get; }

        /// <summary>
        /// プレイリストID
        /// </summary>
        int PlaylistID { get; }

        /// <summary>
        /// 動画ファイルのパス
        /// </summary>
        string FilePath { get; }

        /// <summary>
        /// ID
        /// </summary>
        string NiconicoID { get; }

        /// <summary>
        /// エコノミーファイルであるかどうか
        /// </summary>
        bool IsEconomyFile { get; }

        /// <summary>
        /// メッセージ
        /// </summary>
        ReactiveProperty<string> Message { get; }

        /// <summary>
        /// キャンセルフラグ
        /// </summary>
        ReactiveProperty<bool> IsCanceled { get; }

        /// <summary>
        /// 処理中フラグ
        /// </summary>
        ReactiveProperty<bool> IsProcessing { get; }

        /// <summary>
        /// 完了フラグ
        /// </summary>
        ReactiveProperty<bool> IsDone { get; }

        /// <summary>
        /// 解像度
        /// </summary>
        uint VerticalResolution { get; }

        /// <summary>
        /// DL設定
        /// </summary>
        DownloadSettings DownloadSettings { get; }

        /// <summary>
        /// CT
        /// </summary>
        CancellationToken CancellationToken { get; }

        /// <summary>
        /// 関数を外部からセットする
        /// </summary>
        /// <param name="func"></param>
        void SetFuncctions(Func<IDownloadTask, object, Task> func, Action<int> onWait);

        /// <summary>
        /// DLをキャンセルする
        /// </summary>
        void Cancel();
    }

    /// <summary>
    /// ダウンロードタスク
    /// </summary>
    public class DownloadTask : BindableBase, IDownloadTask, IParallelTask<IDownloadTask>
    {
        public DownloadTask(IListVideoInfo video, DownloadSettings downloadSettings)
        {
            this.DirectoryPath = downloadSettings.FolderPath;
            this.VerticalResolution = downloadSettings.VerticalResolution;
            this.Message = new ReactiveProperty<string>();
            this.IsCanceled = new ReactiveProperty<bool>();
            this.IsProcessing = new ReactiveProperty<bool>();
            this.IsDone = new ReactiveProperty<bool>();
            this.PlaylistID = downloadSettings.PlaylistID;
            this.DownloadSettings = downloadSettings;
            this.IsEconomyFile = video.IsEconomy.Value;
            this.FilePath = video.FileName.Value;
            this._video = video;
            this._cts = new CancellationTokenSource();

            this.OnWait = _ => { };
            this.TaskFunction = async (_, _) => await Task.Delay(0);
        }


        #region field

        private readonly CancellationTokenSource _cts;

        private readonly IListVideoInfo _video;

        #endregion

        #region IParallelTask

        public int Index { get; set; }

        public Func<IDownloadTask, object, Task> TaskFunction { get; private set; }

        public Action<int> OnWait { get; private set; }

        #endregion

        #region Props

        public string DirectoryPath { get; init; }

        public string NiconicoID => this._video.NiconicoId.Value;

        public string Title => this._video.Title.Value;

        public string FilePath { get; init; }

        public bool IsEconomyFile { get; init; }

        public int PlaylistID { get; init; }

        public uint VerticalResolution { get; init; }

        public CancellationToken CancellationToken => this._cts.Token;

        public ReactiveProperty<string> Message { get; init; }

        public ReactiveProperty<bool> IsCanceled { get; init; }

        public ReactiveProperty<bool> IsProcessing { get; init; }

        public ReactiveProperty<bool> IsDone { get; init; }

        public DownloadSettings DownloadSettings { get; init; }

        #endregion

        #region Method

        public void SetFuncctions(Func<IDownloadTask, object, Task> func, Action<int> onWait)
        {
            this.TaskFunction = func;
            this.OnWait = onWait;
        }


        public void Cancel()
        {
            if (this.IsDone.Value) return;
            this._cts.Cancel();
            this.IsCanceled.Value = true;
            this.Message.Value = "DLをキャンセル";
            this.IsProcessing.Value = false;
        }

        #endregion

    }
}
