using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Local.Server.Connection;
using Niconicome.Models.Domain.Local.Server.Core;
using Niconicome.Models.Domain.Utils.Error;
using Niconicome.Models.Helper.Result;

namespace Niconicome.Models.Infrastructure.Network
{
    public class TCPConnectionHandler : ITCPConnectionHandler
    {
        public TCPConnectionHandler(IErrorHandler errorHandler)
        {
            this._errorHandler = errorHandler;
        }

        #region field

        private readonly IErrorHandler _errorHandler;

        #endregion

        #region Method

        public IAttemptResult<IEnumerable<int>> GetOccupiedTCPPorts()
        {

            var connections = IPGlobalProperties.GetIPGlobalProperties();
            var ports = new List<int>();

            try
            {
                var x = connections.GetActiveTcpListeners().Select(c => c.Port);
                ports.AddRange(connections.GetActiveTcpListeners().Select(c => c.Port));
            }
            catch (Exception ex)
            {
                this._errorHandler.HandleError(NetworkError.FailedToGetPortInfo, ex);
                return AttemptResult<IEnumerable<int>>.Fail(this._errorHandler.GetMessageForResult(NetworkError.FailedToGetPortInfo, ex));
            }

            return AttemptResult<IEnumerable<int>>.Succeeded(ports);
        
        }

        #endregion
    }
}
