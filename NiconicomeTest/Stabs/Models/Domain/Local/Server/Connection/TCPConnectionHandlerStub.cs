using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Local.Server.Connection;
using Niconicome.Models.Helper.Result;

namespace NiconicomeTest.Stabs.Models.Domain.Local.Server.Connection
{
    internal class TCPConnectionHandlerStub : ITCPConnectionHandler
    {
        public IAttemptResult<IEnumerable<int>> GetOccupiedTCPPorts()
        {
            return AttemptResult<IEnumerable<int>>.Succeeded(this.OccupiedPorts);
        }

        /// <summary>
        /// ポート情報
        /// </summary>
        public List<int> OccupiedPorts { get; init; } = new();

    }
}
