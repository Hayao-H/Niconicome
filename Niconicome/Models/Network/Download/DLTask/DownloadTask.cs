using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Navigation;
using AngleSharp.Common;
using Niconicome.Models.Const;
using Niconicome.Models.Domain.Local.Store.V2;
using Niconicome.Models.Domain.Niconico.Download;
using Niconicome.Models.Domain.Playlist;
using Niconicome.Models.Domain.Utils.Error;
using Niconicome.Models.Domain.Utils.StringHandler;
using Niconicome.Models.Helper.Result;
using Niconicome.Models.Local.State.MessageV2;
using Niconicome.Models.Network.Download.DLTask.StringContent;
using Niconicome.Models.Playlist.V2.Manager;
using Niconicome.Models.Playlist.V2.Manager.Helper;
using Niconicome.Models.Playlist.VideoList;
using Niconicome.Models.Utils;
using Niconicome.Models.Utils.ParallelTaskV2;
using Niconicome.Models.Utils.Reactive;
using Niconicome.ViewModels;
using Reactive.Bindings;

namespace Niconicome.Models.Network.Download.DLTask;

public interface IDownloadTask : IParallelTask, IDisposable
{
    /// <summary>
    /// メッセージ
    /// </summary>
    IReadonlyBindablePperty<string> Message { get; }

    /// <summary>
    /// 全てのメッセージ
    /// </summary>
    IReadOnlyCollection<string> FullMessage { get; }

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
    /// 保存先パス（個別設定用）
    /// </summary>
    string DirectoryPath { get; set; }

    /// <summary>
    /// ファイル名のフォーマット（個別設定用）
    /// </summary>
    string FileNameFormat { get; set; }

    /// <summary>
    /// 解像度（個別設定用）
    /// </summary>
    uint Resolution { get; set; }

    /// <summary>
    /// 変更監視オブジェクト
    /// </summary>
    Bindables Bindables { get; }

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

public class DownloadTask : ParallelTask, IDownloadTask
{
    public DownloadTask(IPlaylistManager playlistManager, IMessageHandler messageHandler, IContentDownloadHelper contentDownloadHelper, IStringHandler stringHandler, IVideoStore videoStore) : base(_ => Task.CompletedTask, _ => { })
    {
        this._cts = new CancellationTokenSource();
        this._messageHandler = messageHandler;
        this._contentDownloadHelper = contentDownloadHelper;
        this._playlistManager = playlistManager;
        this._stringHandler = stringHandler;
        this._videoStore = videoStore;

        this.TaskFunction = async _ => await this.DownloadAsync();

        this.IsCompleted = new BindableProperty<bool>(false).AddTo(this.Bindables);
        this.IsProcessing = new BindableProperty<bool>(false).AddTo(this.Bindables);
        this.IsCanceled = new BindableProperty<bool>(false).AddTo(this.Bindables);

        this.FullMessage = this._fullMessage.AsReadOnly();
        this.Message = this._message.AsReadOnly();
    }

    #region field

    private IVideoInfo? _video;

    private DownloadSettings? _settings;

    private readonly CancellationTokenSource _cts;

    private readonly IMessageHandler _messageHandler;

    private readonly IContentDownloadHelper _contentDownloadHelper;

    private readonly IPlaylistManager _playlistManager;

    private readonly IVideoStore _videoStore;

    private readonly IStringHandler _stringHandler;

    private readonly List<string> _fullMessage = new();

    private readonly IBindableProperty<string> _message = new BindableProperty<string>(string.Empty);

    #endregion

    #region IParallelTask

    public override void Cancel()
    {
        if (this.IsCompleted.Value) return;
        this._cts.Cancel();
        this.IsCanceled.Value = true;
        this.AppendMessage(this._stringHandler.GetContent(DownloadTaskStringContent.TaskCancelled));
        this.IsProcessing.Value = false;
    }

    #endregion

    #region Props

    public IReadonlyBindablePperty<string> Message { get; init; }

    public IReadOnlyCollection<string> FullMessage { get; init; }

    public IBindableProperty<bool> IsCanceled { get; init; }

    public IBindableProperty<bool> IsProcessing { get; init; }

    public IBindableProperty<bool> IsCompleted { get; init; }

    public Bindables Bindables { get; init; } = new();

    public string DirectoryPath
    {
        get => this._settings?.FolderPath ?? string.Empty;
        set
        {
            if (this._settings is null) return;
            this._settings.FolderPath = value;
        }
    }

    public string FileNameFormat
    {
        get => this._settings?.FileNameFormat ?? string.Empty;
        set
        {
            if (this._settings is null) return;
            this._settings.FileNameFormat = value;
        }
    }

    public uint Resolution
    {
        get => this._settings?.VerticalResolution ?? 1080;
        set
        {
            if (this._settings is null) return;
            this._settings.VerticalResolution = value;
        }
    }

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
        this.Message.Subscribe(x => this._video.Message.Value = x);
        this.AppendMessage(this._stringHandler.GetContent(DownloadTaskStringContent.IsWaiting));
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

        IAttemptResult<IDownloadContext> result = await this._contentDownloadHelper.TryDownloadContentAsync(this._video, this._settings!, msg => this.AppendMessage(msg), this._cts.Token);


        //DL失敗
        if (!result.IsSucceeded || result.Data is null)
        {
            this._messageHandler.AppendMessage(this._stringHandler.GetContent(DownloadTaskStringContent.DownloadFailed, this._video.NiconicoId), LocalConstant.SystemMessageDispacher, ErrorLevel.Error);
            this._messageHandler.AppendMessage(this._stringHandler.GetContent(DownloadTaskStringContent.DownloadFailedDetailed, result.Message ?? string.Empty), LocalConstant.SystemMessageDispacher, ErrorLevel.Error);

            if (!this.IsCanceled.Value)
            {
                this.AppendMessage(this._stringHandler.GetContent(DownloadTaskStringContent.DownloadFailed, this._video!.NiconicoId));
            }

            if (this._settings!.SaveFailedHistory)
            {
                this.AddVideoToSpecialPlaylist(SpecialPlaylists.DownloadFailedHistory);
            }

            this.IsCompleted.Value = true;
            this.IsProcessing.Value = false;
            return AttemptResult.Fail(result.Message);
        }
        else
        //DL成功
        {
            var builder = new StringBuilder();
            builder.Append(this._video!.NiconicoId);
            if (this._settings!.Video || this._settings.Comment) builder.Append("(");
            if (this._settings.Video) builder.Append(this._stringHandler.GetContent(DownloadTaskStringContent.VerticalResolution, result.Data.ActualVerticalResolution));
            if (this._settings.Comment) builder.Append(this._stringHandler.GetContent(DownloadTaskStringContent.CommentCount, result.Data.CommentCount));
            if (this._settings.Video || this._settings.Comment) builder.Append(")");
            string rMessage = builder.ToString();

            this._messageHandler.AppendMessage(this._stringHandler.GetContent(DownloadTaskStringContent.DownloadSucceeded, this._video.NiconicoId), LocalConstant.SystemMessageDispacher, ErrorLevel.Log);

            this.IsSuceeded = true;

            this._video.IsSelected.Value = false;

            this.AppendMessage(this._stringHandler.GetContent(DownloadTaskStringContent.DownloadSucceeded, rMessage));

            if (this._settings.SaveSucceededHistory)
            {
                this.AddVideoToSpecialPlaylist(SpecialPlaylists.DownloadSucceededHistory);
            }

        }

        //コンテクストを履き
        result.Data?.Dispose();

        //DL処理を終了
        this.IsProcessing.Value = false;
        this.IsCompleted.Value = true;
        if (this.IsCanceled.Value)
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

    /// <summary>
    /// 動画を特殊プレイリストに登録
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    private IAttemptResult AddVideoToSpecialPlaylist(SpecialPlaylists type)
    {

        if (this._video is null)
        {
            throw new InvalidOperationException();
        }

        IAttemptResult<IPlaylistInfo> pResult = this._playlistManager.GetSpecialPlaylistByType(type);
        if (!pResult.IsSucceeded || pResult.Data is null)
        {
            return AttemptResult.Fail(pResult.Message);
        }

        IPlaylistInfo playlist = pResult.Data;

        if (this._videoStore.Exist(this._video.NiconicoId, playlist.ID))
        {
            return AttemptResult.Succeeded();
        }

        IAttemptResult cResult = this._videoStore.Create(this._video.NiconicoId, playlist.ID);
        if (!cResult.IsSucceeded)
        {
            return cResult;
        }

        IAttemptResult<IVideoInfo> vResult = this._videoStore.GetVideo(this._video.NiconicoId, playlist.ID);
        if (!vResult.IsSucceeded || vResult.Data is null)
        {
            return AttemptResult.Fail(vResult.Message);
        }

        return playlist.AddVideo(vResult.Data);



    }

    /// <summary>
    /// メッセージを追記
    /// </summary>
    /// <param name="message"></param>
    private void AppendMessage(string message)
    {
        this._message.Value = message;
        this._fullMessage.Add(message);
        this.Bindables.RaiseChange();
    }

    #endregion

    public void Dispose()
    {
        this.Bindables.Dispose();
    }

}

