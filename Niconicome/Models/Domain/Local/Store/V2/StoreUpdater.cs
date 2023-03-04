using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Helper.Result;

namespace Niconicome.Models.Domain.Local.Store.V2
{
    public interface IStoreUpdater<T>
    {
        /// <summary>
        /// データを更新
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        IAttemptResult Update(T data);
    }
}
