using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using Niconicome.Models.Domain.Local.Settings;
using Niconicome.Models.Domain.Utils.Error;
using Niconicome.Models.Helper.Result;
using Const = Niconicome.Models.Const;

namespace Niconicome.Models.Domain.Local.Server.Core
{
    public interface IPortHandler
    {
        /// <summary>
        /// 設定値を取得する
        /// </summary>
        /// <returns></returns>
        int GetSettingValue();

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
        public PortHandler(IErrorHandler errorHandler, ISettingsContainer settingsContainer)
        {
            this._errorHandler = errorHandler;
            this._settingsContainer = settingsContainer;
        }

        #region field

        private readonly IErrorHandler _errorHandler;

        private readonly ISettingsContainer _settingsContainer;

        #endregion

        #region Method

        public int GetSettingValue()
        {
            IAttemptResult<int> result = this._settingsContainer.GetOnlyValue(SettingNames.LocalServerPort, Const::NetConstant.DefaultServerPort);
            if (!result.IsSucceeded)
            {
                return Const::NetConstant.DefaultServerPort;
            }
            else
            {
                return result.Data;
            }
        }


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
