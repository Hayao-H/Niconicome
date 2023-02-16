using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Niconicome.Models.Const;
using Niconicome.Models.Domain.Niconico.Download;
using Niconicome.Models.Domain.Playlist;
using Niconicome.Models.Domain.Utils.Error;
using Niconicome.Models.Domain.Utils.StringHandler;
using Niconicome.Models.Helper.Result;
using Niconicome.Models.Local.State.MessageV2;
using Niconicome.Models.Network.Download.DLTask.StringContent;
using Niconicome.Models.Playlist.V2.Manager;
using Niconicome.Models.Playlist.VideoList;
using Niconicome.Models.Utils;
using Niconicome.Models.Utils.Reactive;
using Niconicome.ViewModels;
using Reactive.Bindings;

namespace Niconicome.Models.Network.Download.DLTask;

public interface IDownloadTask : IParallelTask<IDownloadTask>
{
    /// <summary>
    /// メッセージ
    /// </summary>
    IBindableProperty<string> Message { get; }

    /// <summary>
    /// キャンセルフラグ
    /// </summary>
    IBindableProperty<bool> IsCanceled { get; }

    /// <summary>
    /// 処理中フラグ
    /// </summary>
    IBindableProperty<bool> IsProcessing { get; }

    /// <summary>
    /// 完了フラグ
    /// </summary>
    IBindableProperty<bool> IsCompleted { get; }

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
    IBindableProperty<uint> Resolution { get; }

    /// <summary>
    /// 初期化する
    /// </summary>
    /// <param name="video"></param>
    /// <param name="settings"></param>
    void Initialize(IVideoInfo video, DownloadSettings settings);

    /// <summary>
    /// ダウンロードを開始する
    /// </summary>
    /// <returns></returns>
    Task<IAttemptResult> DownloadAsync();
}

public class DownloadTask : BindableBase, IDownloadTask, IParallelTask<IDownloadTask>
{
    public DownloadTask(IPlaylistManager playlistManager, IMessageHandler messageHandler, IContentDownloadHelper contentDownloadHelper, IVideoListContainer container,IStringHandler stringHandler)
    {
        this._cts = new CancellationTokenSource();
        this._messageHandler = messageHandler;
        this._contentDownloadHelper = contentDownloadHelper;
        this._container = container;
        this._playlistManager = playlistManager;
        this._stringHandler = stringHandler;

        this.OnWait = _ => { };
        this.TaskFunction = async (_, _) => await this.DownloadAsync();

        this.IsCompleted = new BindableProperty<bool>(false);
        this.IsProcessing = new BindableProperty<bool>(false);
        this.IsCanceled = new BindableProperty<bool>(false);
        this.Message = new BindableProperty<string>(string.Empty);

        this.Resolution.Subscribe(value =>
        {
            if (this._settings is not null)
            {
                this._settings.VerticalResolution = value;
            }
        });
    }


    #region field


    private IVideoInfo? _video;

    private DownloadSettings? _settings;

    private readonly CancellationTokenSource _cts;

    private readonly IMessageHandler _messageHandler;

    private readonly IContentDownloadHelper _contentDownloadHelper;

    private readonly IPlaylistManager _playlistManager;

    private readonly IVideoListContainer _container;

    private readonly IStringHandler _stringHandler;

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
        this.Message.Value = this._stringHandler.GetContent(DownloadTaskStringContent.TaskCancelled);
        this.IsProcessing.Value = false;
    }

    #endregion

    #region Props

    public IBindableProperty<string> Message { get; private set; }

    public IBindableProperty<uint> Resolution { get; init; } = new BindableProperty<uint>(0);

    public IBindableProperty<bool> IsCanceled { get; init; }

    public IBindableProperty<bool> IsProcessing { get; init; }

    public IBindableProperty<bool> IsCompleted { get; init; }

    public bool IsSuceeded { get; private set; }

    public string NiconicoID => this._video?.NiconicoId ?? string.Empty;

    public string Title => this._video?.Title ?? string.Empty;

    public int PlaylistID => this._settings?.PlaylistID ?? -1;

    #endregion

    #region Method


    public void Initialize(IVideoInfo video, DownloadSettings settings)
    {
        this._video = video;
        this._settings = settings;
        this.Resolution.Value = settings.VerticalResolution;
        this.Message.Subscribe(m => this._video.Message.Value = m);
        this.Message.Value = this._stringHandler.GetContent(DownloadTaskStringContent.IsWaiting);
    }

    public async Task<IAttemptResult> DownloadAsync()
    {
        //完了・キャンセル時は処理を中止
        if (this.IsCanceled.Value) return AttemptResult.Fail(this._stringHandler.GetContent(DownloadTaskStringContent.AlreadyCancelled));
        if (this.IsCompleted.Value || this.IsProcessing.Value) return AttemptResult.Fail(this._stringHandler.GetContent(DownloadTaskStringContent.AlreadyDownloaded));

        //初期化されていなければ処理をキャンセル
        if (!this.CheckIfInitialized()) return AttemptResult.Fail(this._stringHandler.GetContent(DownloadTaskStringContent.NotInitialized));

        //DLを開始
        this._messageHandler.AppendMessage(this._stringHandler.GetContent(DownloadTaskStringContent.DownloadStarted, this._video!.NiconicoId), LocalConstant.SystemMessageDispacher, ErrorLevel.Log);
        this.IsProcessing.Value = true;

        IAttemptResult<IDownloadContext> result = await this._contentDownloadHelper.TryDownloadContentAsync(this._video, this._settings!, msg => this.Message.Value = msg, this._cts.Token);


        //DL失敗
        if (!result.IsSucceeded || result.Data is null)
        {
            this._messageHandler.AppendMessage(this._stringHandler.GetContent(DownloadTaskStringContent.DownloadFailed, this._video.NiconicoId), LocalConstant.SystemMessageDispacher, ErrorLevel.Error);
            this._messageHandler.AppendMessage(this._stringHandler.GetContent(DownloadTaskStringContent.DownloadFailedDetailed, result.Message ?? string.Empty), LocalConstant.SystemMessageDispacher, ErrorLevel.Error);
            this.Message.Value = this._stringHandler.GetContent(DownloadTaskStringContent.DownloadFailedMessage);

            if (this._settings!.SaveFailedHistory)
            {
                IAttemptResult<IPlaylistInfo> pResult = this._playlistManager.GetSpecialPlaylistByType(SpecialPlaylists.DownloadFailedHistory);
                if (pResult.IsSucceeded && pResult.Data is not null)
                {
                    pResult.Data.AddVideo(this._video);
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
            if (this._settings.Video) builder.Append(this._stringHandler.GetContent(DownloadTaskStringContent.VerticalResolution, result.Data.ActualVerticalResolution));
            if (this._settings.Comment) builder.Append(this._stringHandler.GetContent(DownloadTaskStringContent.CommentCount, result.Data.CommentCount));
            if (this._settings.Video || this._settings.Comment) builder.Append(")");
            string rMessage = builder.ToString();

            this._messageHandler.AppendMessage(this._stringHandler.GetContent(DownloadTaskStringContent.DownloadSucceeded, this._video.NiconicoId), LocalConstant.SystemMessageDispacher, ErrorLevel.Log);

            //ファイル情報を更新
            //他のプレイリストを開いていると書き換えでエラーが起きるかもなのでちゃんと戻す
            if (!string.IsNullOrEmpty(result.Data.FileName))
            {
                this._video.FilePath = result.Data.FileName;
                this._video.IsDownloaded.Value = true;
            }

            this._video.IsSelected.Value = false;

            this.Message.Value = this._stringHandler.GetContent(DownloadTaskStringContent.DownloadSucceeded, rMessage);

            if (this._settings.SaveSucceededHistory)
            {
                IAttemptResult<IPlaylistInfo> pResult = this._playlistManager.GetSpecialPlaylistByType(SpecialPlaylists.DownloadSucceededHistory);
                if (pResult.IsSucceeded && pResult.Data is not null)
                {
                    pResult.Data.AddVideo(this._video);
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
            return AttemptResult.Fail(this._stringHandler.GetContent(DownloadTaskStringContent.TaskCancelled));
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

