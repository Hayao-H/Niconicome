using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Local.Addons.Core;
using Niconicome.Models.Domain.Local.Addons.Core.Utils;

namespace Niconicome.Models.Domain.Local.Addons.API.Tab
{
    public interface ITabInfomation
    {
        /// <summary>
        /// アドオン情報
        /// </summary>
        AddonInfomation AddonInfomation { get; }

        /// <summary>
        /// タイトル
        /// </summary>
        string Title { get; }

        /// <summary>
        /// アクセス可能であるかどうかを判断
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        bool CanAccess(string url);

        /// <summary>
        /// 初期化
        /// </summary>
        /// <param name="infomation"></param>
        void Initialize(AddonInfomation infomation, string title);
    }

    public class TabInfomation : ITabInfomation
    {
        public TabInfomation(IHostPermissionsHandler hostPermissionsHandler)
        {
            this._hostPermissionsHandler = hostPermissionsHandler;
        }

        #region field

        private readonly IHostPermissionsHandler _hostPermissionsHandler;

        private bool isInitialized;

        #endregion

        #region Props

        public AddonInfomation AddonInfomation { get; private set; } = new();

        public string Title { get; private set; } = string.Empty;


        #endregion

        #region Methods

        public void Initialize(AddonInfomation infomation, string title)
        {
            if (this.isInitialized) return;

            this.AddonInfomation = infomation;
            this._hostPermissionsHandler.Initialize(infomation.HostPermissions);
            this.Title = title;

            this.isInitialized = true;
        }

        public bool CanAccess(string url)
        {
            return this._hostPermissionsHandler.CanAccess(url);
        }


        #endregion
    }
}
