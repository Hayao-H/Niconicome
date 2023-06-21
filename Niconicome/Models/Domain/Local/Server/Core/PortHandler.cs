using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using Niconicome.Models.Domain.Local.Server.Connection;
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
        public PortHandler(IErrorHandler errorHandler, ISettingsContainer settingsContainer, ITCPConnectionHandler connectionHandler)
        {
            this._errorHandler = errorHandler;
            this._settingsContainer = settingsContainer;
            this._connectionHandler = connectionHandler;
        }

        #region field

        private readonly IErrorHandler _errorHandler;

        private readonly ISettingsContainer _settingsContainer;

        private readonly ITCPConnectionHandler _connectionHandler;

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
            IAttemptResult<IEnumerable<int>> ports = this._connectionHandler.GetOccupiedTCPPorts();
            if (!ports.IsSucceeded || ports.Data is null)
            {
                return AttemptResult<IEnumerable<int>>.Fail(ports.Message);
            }

            var available = new List<int>();
            foreach (var p in Enumerable.Range(1024, 65536 - 1024 + 1))
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

            IAttemptResult<IEnumerable<int>> ports = this._connectionHandler.GetOccupiedTCPPorts();
            if (!ports.IsSucceeded || ports.Data is null)
            {
                return false;
            }

            return !ports.Data.Contains(port);
        }

        #endregion
    }
}
