using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Local.Settings;

namespace Niconicome.Models.Domain.Local.Addons.Core.V2.Utils
{
    public interface IAddonSettingsHandler
    {
        bool IsDevelopperMode { get; set; }
    }

    public class AddonSettingsHandler : IAddonSettingsHandler
    {
        public AddonSettingsHandler(ILocalSettingsContainer settingsContainer)
        {
            this._settingsContainer = settingsContainer;
        }

        #region field

        private readonly ILocalSettingsContainer _settingsContainer;

        #endregion

        #region Props

        /// <summary>
        /// 開発者モード
        /// </summary>
        public bool IsDevelopperMode
        {
            get => this._settingsContainer.GetReactiveBoolSetting(SettingsEnum.IsDevMode).Value;
            set => this._settingsContainer.GetReactiveBoolSetting(SettingsEnum.IsDevMode).Value = value;
        }


        #endregion
    }
}
