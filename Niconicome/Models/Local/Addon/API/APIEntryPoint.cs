﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Printing;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Local.Addons.Core;
using Niconicome.Models.Domain.Local.Addons.Core.Permisson;
using Niconicome.Models.Domain.Niconico.Video.Infomations;
using Niconicome.Models.Domain.Niconico.Watch;
using Niconicome.Models.Local.Addon.API.Local.IO;
using Niconicome.Models.Local.Addon.API.Net.Hooks;
using Niconicome.Models.Local.Addon.API.Net.Http.Fetch;

namespace Niconicome.Models.Local.Addon.API
{
    public interface IAPIEntryPoint
    {
        /// <summary>
        /// 出力
        /// </summary>
        IOutput? output { get; }

        /// <summary>
        /// Hooks API
        /// </summary>
        IHooks? hooks { get; }

        void Initialize(AddonInfomation infomation, IJavaScriptExecuter engine);
    }

    public class APIEntryPoint : IAPIEntryPoint
    {
        public APIEntryPoint(IOutput output,IHooks hooks)
        {
            this.output = output;
            this.hooks= hooks;
        }

        #region Props

        public IOutput? output { get; private set; }

        public IHooks? hooks { get; private set; }

        #endregion

        #region methods

        public void Initialize(AddonInfomation infomation, IJavaScriptExecuter engine)
        {
            if (!infomation.HasPermission(PermissionNames.Output))
            {
                this.output = null;
            }
            else
            {
                this.output!.SetInfo(infomation);
            }

            if (infomation.HasPermission(PermissionNames.Hooks))
            {
                engine.AddHostType(nameof(DmcInfo), typeof(DmcInfo));
                engine.AddHostType(nameof(SessionInfo), typeof(SessionInfo));
            }
            else
            {
                this.hooks = null;
            }
        }


        #endregion
    }
}
