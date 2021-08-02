using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Local.Addons.Core;
using Niconicome.Models.Domain.Local.Addons.Core.Engine;
using Niconicome.ViewModels.Mainpage;

namespace Niconicome.Models.Local.Addon.API.Local.IO
{
    public interface ILog
    {
        /// <summary>
        /// エラー処理
        /// </summary>
        /// <param name="message"></param>
        void error(string message);

        /// <summary>
        /// 初期化する
        /// </summary>
        /// <param name="info"></param>
        void Initialize(AddonInfomation info);
    }

    class Log : ILog
    {

        public Log(IAddonLogger logger)
        {
            this.logger = logger;
        }

        #region field

        private readonly IAddonLogger logger;

        private AddonInfomation? info;

        #endregion

        public void error(string message)
        {
            if (this.info is null) throw new InvalidOperationException();
            this.logger.Error(message, this.info.Name.Value);
        }

        public void Initialize(AddonInfomation info)
        {
            this.info = info;
        }


    }
}
