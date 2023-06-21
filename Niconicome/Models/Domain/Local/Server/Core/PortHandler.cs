using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Utils.Error;
using Niconicome.Models.Helper.Result;

namespace Niconicome.Models.Domain.Local.Server.Core
{
    public interface IPortHandler
    {
        /// <summary>
        /// 空きポートであるかどうか調べる
        /// </summary>
        /// <param name="port"></param>
        /// <returns></returns>
        bool IsPortAvailable(int port);

        /// <summary>
        /// 空きポートを取得
        /// </summary>
        /// <returns></returns>
        IAttemptResult<IEnumerable<int>> GetAvailablePorts();
    }

    public class PortHandler : IPortHandler
    {
        public PortHandler(IErrorHandler errorHandler)
        {
            this._errorHandler = errorHandler;
        }

        #region field

        public IErrorHandler _errorHandler;

        #endregion

        #region Method

        public IAttemptResult<IEnumerable<int>> GetAvailablePorts()
        {
            IAttemptResult<List<int>> ports = this.GetUnAvailablePorts();
            if (!ports.IsSucceeded || ports.Data is null)
            {
                return AttemptResult<IEnumerable<int>>.Fail(ports.Message);
            }

            var available = new List<int>();
            foreach (var p in Enumerable.Range(1024, 65536))
            {
                if (ports.Data.Any(x => x == p))
                {
                    continue;
                }

                available.Add(p);

            }

            return AttemptResult<IEnumerable<int>>.Succeeded(available);

        }

        public bool IsPortAvailable(int port)
        {

            IAttemptResult<List<int>> ports = this.GetUnAvailablePorts();
            if (!ports.IsSucceeded || ports.Data is null)
            {
                return false;
            }

            return !ports.Data.Contains(port);
        }

        #endregion

        #region private

        private IAttemptResult<List<int>> GetUnAvailablePorts()
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
                this._errorHandler.HandleError(ServerError.FailedToGetPortInfo, ex);
                return AttemptResult<List<int>>.Fail(this._errorHandler.GetMessageForResult(ServerError.FailedToGetPortInfo, ex));
            }

            return AttemptResult<List<int>>.Succeeded(ports);
        }

        #endregion
    }
}
