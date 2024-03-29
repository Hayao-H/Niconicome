﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Local.Addons.Core.V2.Engine.Infomation;
using Niconicome.Models.Domain.Local.Addons.Core.V2.Utils;

namespace Niconicome.Models.Domain.Local.Addons.API.Tab
{
    public interface ITabInfomation
    {
        /// <summary>
        /// アドオン情報
        /// </summary>
        IAddonInfomation AddonInfomation { get; }

        /// <summary>
        /// タイトル
        /// </summary>
        string Title { get; }

        /// <summary>
        /// ID
        /// </summary>
        string ID { get; }

        /// <summary>
        /// タブの位置情報
        /// </summary>
        TabType TabType { get; }

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
        void Initialize(IAddonInfomation infomation, string title, TabType tabType);
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

        public IAddonInfomation AddonInfomation { get; private set; } = new AddonInfomation();

        public string Title { get; private set; } = string.Empty;

        public TabType TabType { get; private set; }

        public string ID { get; init; } = Guid.NewGuid().ToString("D");

        #endregion

        #region Methods

        public void Initialize(IAddonInfomation infomation, string title,TabType tabType)
        {
            if (this.isInitialized) return;

            this.AddonInfomation = infomation;
            this._hostPermissionsHandler.Initialize(infomation.HostPermissions);
            this.Title = title;
            this.TabType = tabType;

            this.isInitialized = true;
        }

        public bool CanAccess(string url)
        {
            return this._hostPermissionsHandler.CanAccess(url);
        }


        #endregion
    }
}
