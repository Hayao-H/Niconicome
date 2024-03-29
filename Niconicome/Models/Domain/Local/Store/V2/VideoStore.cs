﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Playlist;
using Niconicome.Models.Helper.Result;

namespace Niconicome.Models.Domain.Local.Store.V2
{
    public interface IVideoStore : IStoreUpdater<IVideoInfo>
    {
        /// <summary>
        /// ニコニコのIDで動画を取得
        /// </summary>
        /// <param name="niconicoID"></param>
        /// <param name="playlistID"></param>
        /// <returns></returns>
        IAttemptResult<IVideoInfo> GetVideo(string niconicoID, int playlistID);

        /// <summary>
        /// 動画を取得
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="playlistID"></param>
        /// <returns></returns>
        IAttemptResult<IVideoInfo> GetOnlySharedVideoVideo(int ID);

        /// <summary>
        /// 全ての動画を取得
        /// </summary>
        /// <returns></returns>
        IAttemptResult<IEnumerable<IVideoInfo>> GetAllVideos();

        /// <summary>
        /// 動画を作成
        /// </summary>
        /// <param name="video"></param>
        /// <returns></returns>
        IAttemptResult<int> Create(string niconicoID);

        /// <summary>
        /// 動画を作成
        /// </summary>
        /// <param name="niconicoID"></param>
        /// <returns></returns>
        IAttemptResult Create(string niconicoID, int playlistID);

        /// <summary>
        /// 動画を削除
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="playlistID"></param>
        /// <returns></returns>
        IAttemptResult Delete(int ID, int playlistID);

        /// <summary>
        /// 全ての動画を削除
        /// </summary>
        /// <returns></returns>
        IAttemptResult Clear();

        /// <summary>
        /// キャッシュをクリア
        /// </summary>
        void Flush();

        /// <summary>
        /// 指定したIDで動画の存在を確認
        /// </summary>
        /// <param name="niconicoID"></param>
        /// <param name="playlistID"></param>
        /// <returns></returns>
        bool Exist(string niconicoID, int playlistID);
    }
}
