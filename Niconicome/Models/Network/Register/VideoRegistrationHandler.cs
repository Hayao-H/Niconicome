using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Helper.Result;
using Niconicome.Models.Local.State;
using Niconicome.Models.Playlist;
using Niconicome.Models.Playlist.VideoList;
using Reactive.Bindings;

namespace Niconicome.Models.Network.Register
{
    public interface IVideoRegistrationHandler
    {
        /// <summary>
        /// 動画を登録する
        /// </summary>
        /// <param name="inputText">入力値</param>
        /// <param name="extractIDsFromText">テキストからIDを抽出(クリップボード監視等)</param>
        /// <returns></returns>
        Task<IAttemptResult<IEnumerable<IListVideoInfo>>> ResgisterVideoAsync(string inputText, bool extractIDsFromText);

        /// <summary>
        /// 処理中フラグ
        /// </summary>
        ReadOnlyReactivePropertySlim<bool> IsProcessing { get; }
    }

    public class VideoRegistrationHandler : IVideoRegistrationHandler
    {
        public VideoRegistrationHandler(IVideoIDHandler idHandler, IMessageHandler messageHandler, ICurrent current, IVideoListContainer container)
        {
            this.IsProcessing = idHandler.IsProcessing.ToReadOnlyReactivePropertySlim();
            this._idHandler = idHandler;
            this._messageHandler = messageHandler;
            this._currrent = current;
            this._container = container;
        }

        #region field

        private readonly IMessageHandler _messageHandler;

        private readonly IVideoIDHandler _idHandler;

        private readonly ICurrent _currrent;

        private readonly IVideoListContainer _container;

        #endregion

        #region Methods

        public async Task<IAttemptResult<IEnumerable<IListVideoInfo>>> ResgisterVideoAsync(string inputText, bool extractIDsFromText)
        {
            return await this.RegisterVideosInternalAsync(inputText, extractIDsFromText);

        }

        #endregion

        #region Props

        public ReadOnlyReactivePropertySlim<bool> IsProcessing { get; init; }

        #endregion

        #region private

        private async Task<IAttemptResult<IEnumerable<IListVideoInfo>>> RegisterVideosInternalAsync(string inputText, bool extractFromText)
        {

            //確認
            if (this.IsProcessing.Value)
            {
                return AttemptResult<IEnumerable<IListVideoInfo>>.Fail("現在処理中です。");
            }

            int? playlistID = this._currrent.SelectedPlaylist.Value?.Id;

            IAttemptResult<IEnumerable<IListVideoInfo>> fetchResult = await this._idHandler.GetVideoListInfosAsync(inputText, m => this._messageHandler.AppendMessage(m), extractFromText);

            if (!fetchResult.IsSucceeded || fetchResult.Data is null)
            {
                return AttemptResult<IEnumerable<IListVideoInfo>>.Fail($"動画の取得に失敗しました。（詳細：{fetchResult.Message}）");
            }

            IAttemptResult addResult = this._container.AddRange(fetchResult.Data, playlistID);

            if (!addResult.IsSucceeded)
            {
                return AttemptResult<IEnumerable<IListVideoInfo>>.Fail($"動画の追加処理に失敗しました。（詳細：{addResult.Message}）");
            }
            else
            {
                return AttemptResult<IEnumerable<IListVideoInfo>>.Succeeded(fetchResult.Data);
            }
        }

        #endregion
    }
}
