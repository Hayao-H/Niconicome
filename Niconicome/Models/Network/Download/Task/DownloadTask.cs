using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Niconico.Download;
using Niconicome.Models.Helper.Result;
using Niconicome.Models.Local.State;
using Niconicome.Models.Playlist;
using Niconicome.Models.Playlist.Playlist;
using Niconicome.Models.Playlist.VideoList;
using Niconicome.Models.Utils;
using Niconicome.ViewModels;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace Niconicome.Models.Network.Download.DLTask;

public interface IDownloadTask : IParallelTask<IDownloadTask>
{
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
    ReactiveProperty<bool> IsCompleted { get; }

    /// <summary>
    /// DL成功フラグ
    /// </summary>
    bool IsSuceeded { get; }

    /// <summary>
    /// プレイリストID（キュー移動時の判断に使用）
    /// </summary>
    int PlaylistID { get; }

    /// <summary>
    /// 動画ID（情報表示に使用）
    /// </summary>
    string NiconicoID { get; }

    /// <summary>
    /// 動画タイトル（情報表示に使用）
    /// </summary>
    string Title { get; }

    /// <summary>
    /// 解像度（個別設定用）
    /// </summary>
    ReactiveProperty<uint> Resolution { get; }

    /// <summary>
    /// 初期化する
    /// </summary>
    /// <param name="video"></param>
    /// <param name="settings"></param>
    void Initialize(IListVideoInfo video, DownloadSettings settings);

    /// <summary>
    /// ダウンロードを開始する
    /// </summary>
    /// <returns></returns>
    Task<IAttemptResult> DownloadAsync();
}

public class DownloadTask : BindableBase, IDownloadTask, IParallelTask<IDownloadTask>
{
    public DownloadTask(IPlaylistHandler playlistHandler, IMessageHandler messageHandler, IContentDownloadHelper contentDownloadHelper, IVideoListContainer container, IVideosUnchecker unchecker)
    {
        this._cts = new CancellationTokenSource();
        this._messageHandler = messageHandler;
        this._contentDownloadHelper = contentDownloadHelper;
        this._playlistHandler = playlistHandler;
        this._container = container;
        this._unchecker = unchecker;

        this.OnWait = _ => { };
        this.TaskFunction = async (_, _) => await this.DownloadAsync();

        this.IsCompleted = new ReactiveProperty<bool>().AddTo(this.disposables);
        this.IsProcessing = new ReactiveProperty<bool>().AddTo(this.disposables);
        this.IsCanceled = new ReactiveProperty<bool>().AddTo(this.disposables);
        this.Message = new ReactiveProperty<string>("未初期化").AddTo(this.disposables);

        this.Resolution.Subscribe(value =>
        {
            if (this._settings is not null)
            {
                this._settings.VerticalResolution = value;
            }
        });
    }


    #region field


    private IListVideoInfo? _video;

    private DownloadSettings? _settings;

    private readonly CancellationTokenSource _cts;

    private readonly IMessageHandler _messageHandler;

    private readonly IContentDownloadHelper _contentDownloadHelper;

    private readonly IPlaylistHandler _playlistHandler;

    private readonly IVideoListContainer _container;

    private readonly IVideosUnchecker _unchecker;

    #endregion

    #region IParallelTask

    public int Index { get; set; }

    public Func<IDownloadTask, object, Task> TaskFunction { get; init; }

    public Action<int> OnWait { get; private set; }

    public void Cancel()
    {
        if (this.IsCompleted.Value) return;
        this._cts.Cancel();
        this.IsCanceled.Value = true;
        this.Message.Value = "DLをキャンセル";
        this.IsProcessing.Value = false;
    }

    #endregion

    #region Props

    public ReactiveProperty<string> Message { get; private set; } 

    public ReactiveProperty<uint> Resolution { get; init; } = new();

    public ReactiveProperty<bool> IsCanceled { get; init; }

    public ReactiveProperty<bool> IsProcessing { get; init; }

    public ReactiveProperty<bool> IsCompleted { get; init; }

    public bool IsSuceeded { get; private set; }

    public string NiconicoID => this._video?.NiconicoId.Value ?? string.Empty;

    public string Title => this._video?.Title.Value ?? string.Empty;

    public int PlaylistID => this._settings?.PlaylistID ?? -1;

    #endregion

    #region Method


    public void Initialize(IListVideoInfo video, DownloadSettings settings)
    {
        this._video = video;
        this._settings = settings;
        this.Resolution.Value = settings.VerticalResolution;
        this.Message.Subscribe(m => this._video.Message.Value = m);
        this.Message.Value = "待機中...";
    }

    public async Task<IAttemptResult> DownloadAsync()
    {
        //完了・キャンセル時は処理を中止
        if (this.IsCanceled.Value) return AttemptResult.Fail("キャンセルされています。");
        if (this.IsCompleted.Value || this.IsProcessing.Value) return AttemptResult.Fail("すでにダウンロードされています。");

        //初期化されていなければ処理をキャンセル
        if (!this.CheckIfInitialized()) return AttemptResult.Fail("初期化されていません。");

        //DLを開始
        this._messageHandler.AppendMessage($"{this._video!.NiconicoId.Value}のダウンロード処理を開始しました。");
        this.IsProcessing.Value = true;

        IAttemptResult<IDownloadContext> result = await this._contentDownloadHelper.TryDownloadContentAsync(this._video, this._settings!, msg => this.Message.Value = msg, this._cts.Token);


        //DL失敗
        if (!result.IsSucceeded || result.Data is null)
        {
            this._messageHandler.AppendMessage($"{this._video.NiconicoId.Value}のダウンロードに失敗しました。");
            this._messageHandler.AppendMessage($"詳細: {result.Message}");
            this.Message.Value = "DL失敗";

            if (this._settings!.SaveFailedHistory)
            {
                IAttemptResult<ITreePlaylistInfo> pResult = this._playlistHandler.GetSpecialPlaylist(SpecialPlaylistTypes.DLFailedHistory);
                if (pResult.IsSucceeded && pResult.Data is not null)
                {
                    this._playlistHandler.WireVideoToPlaylist(this._video.Id.Value, pResult.Data.Id);
                }
            }

            this.IsCompleted.Value = true;
            this.IsProcessing.Value = false;
            return AttemptResult.Fail(result.Message);
        }
        else
        //DL成功
        {
            var builder = new StringBuilder();
            if (this._settings!.Video || this._settings.Comment) builder.Append("(");
            if (this._settings.Video) builder.Append($"vertical:{result.Data.ActualVerticalResolution}px ");
            if (this._settings.Comment) builder.Append($"{result.Data.CommentCount}コメ");
            if (this._settings.Video || this._settings.Comment) builder.Append(")");
            string rMessage = builder.ToString();
            this._messageHandler.AppendMessage($"{this._video.NiconicoId.Value}のダウンロードに成功しました。");

            //ファイル情報を更新
            //他のプレイリストを開いていると書き換えでエラーが起きるかもなのでちゃんと戻す
            string tmp = this._video.FileName.Value;

            if (!string.IsNullOrEmpty(result.Data.FileName))
            {
                this._video.FileName.Value = result.Data.FileName;
            }
            this._container.Update(this._video, this._settings.PlaylistID);

            this._video.FileName.Value = tmp;

            this._unchecker.Uncheck(this._video.NiconicoId.Value, this._settings.PlaylistID);

            this.Message.Value = $"ダウンロード完了{rMessage}";

            if (this._settings.SaveSucceededHistory)
            {
                IAttemptResult<ITreePlaylistInfo> pResult = this._playlistHandler.GetSpecialPlaylist(SpecialPlaylistTypes.DLSucceedeeHistory);
                if (pResult.IsSucceeded && pResult.Data is not null)
                {
                    this._playlistHandler.WireVideoToPlaylist(this._video.Id.Value, pResult.Data.Id);
                }
            }

        }

        //コンテクストを履き
        result.Data?.Dispose();

        //DL処理を終了
        this.IsProcessing.Value = false;
        this.IsCompleted.Value = true;
        if (!this.IsCanceled.Value)
        {
            this.IsSuceeded = true;
        }
        else
        {
            return AttemptResult.Fail("キャンセルされました。");
        }

        return AttemptResult.Succeeded();
    }


    #endregion

    #region private

    /// <summary>
    /// 初期化されているかどうかを確認
    /// </summary>
    /// <returns></returns>
    private bool CheckIfInitialized()
    {
        return this._video is not null && this._settings is not null;
    }

    #endregion

}

