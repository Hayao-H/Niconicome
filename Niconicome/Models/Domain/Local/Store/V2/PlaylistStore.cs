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
        /// プレイリストを作成
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        IAttemptResult<int> Create(string name);

        /// <summary>
        /// プレイリストを削除
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        IAttemptResult Delete(int ID);
    }
}
