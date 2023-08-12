using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Helper.Result;

namespace Niconicome.Models.Domain.Local.Server.Connection
{
    public interface ITCPConnectionHandler
    {
        /// <summary>
        /// 使用中のTCPポートを取得
        /// </summary>
        /// <returns></returns>
        IAttemptResult<IEnumerable<int>> GetOccupiedTCPPorts();
    }
}
