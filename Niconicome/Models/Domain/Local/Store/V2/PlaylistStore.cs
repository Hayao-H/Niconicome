using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Playlist;
using Niconicome.Models.Helper.Result;

namespace Niconicome.Models.Domain.Local.Store.V2
{
    public interface IPlaylistStore : IStoreUpdater<IPlaylistInfo>
    {
        /// <summary>
        /// 全てのプレイリストを取得
        /// </summary>
        /// <returns></returns>
        IAttemptResult<IReadOnlyList<IPlaylistInfo>> GetAllPlaylist();

        /// <summary>
        /// 指定したIDのプレイリストを取得する
        /// </summary>
        /// <returns></returns>
        IAttemptResult<IPlaylistInfo> GetPlaylist(int ID);

        /// <summary>
        /// プレイリストの種類を指定して取得
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        IAttemptResult<IPlaylistInfo> GetPlaylistByType(PlaylistType type);

        /// <summary>
        /// プレイリストを作成
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        IAttemptResult<int> Create(string name);

        /// <summary>
        /// 全てのプレイリストを削除
        /// </summary>
        /// <returns></returns>
        IAttemptResult Clear();

        /// <summary>
        /// プレイリストを削除
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        IAttemptResult Delete(int ID);

        /// <summary>
        /// プレイリストの存在を確認する
        /// </summary>
        /// <param name="playlistType"></param>
        /// <returns></returns>
        bool Exist(PlaylistType playlistType);
    }
}
