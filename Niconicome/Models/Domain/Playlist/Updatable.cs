using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Helper.Result;

namespace Niconicome.Models.Domain.Playlist
{
    public interface IUpdatable
    {
        /// <summary>
        /// 自動更新の有効・無効
        /// </summary>
        bool IsAutoUpdateEnabled { get; set; }
    }
}
