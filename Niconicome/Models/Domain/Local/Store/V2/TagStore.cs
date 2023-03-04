using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Playlist;
using Niconicome.Models.Helper.Result;

namespace Niconicome.Models.Domain.Local.Store.V2
{
    public interface ITagStore : IStoreUpdater<ITagInfo>
    {
        /// <summary>
        /// 指定した名前のタグを取得する
        /// </summary>
        /// <returns></returns>
        IAttemptResult<ITagInfo> GetTag(string tag);

        /// <summary>
        /// 指定したIDのタグを取得する
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        IAttemptResult<ITagInfo> GetTag(int id);

        /// <summary>
        /// タグを作成
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        IAttemptResult Create(string tag);

        /// <summary>
        /// タグを削除
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        IAttemptResult Delete(int ID);

        /// <summary>
        /// タグの存在有無を確認
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        bool Exist(string Name);
    }
}
