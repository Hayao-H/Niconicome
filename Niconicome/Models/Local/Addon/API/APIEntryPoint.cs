using System;
using System.Collections.Generic;
using System.Linq;
using System.Printing;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Local.Addons.Core;
using Niconicome.Models.Domain.Local.Addons.Core.V2.Permisson;
using Niconicome.Models.Domain.Niconico.Video.Infomations;
using Niconicome.Models.Domain.Niconico.Watch;
using Niconicome.Models.Local.Addon.API.Local.IO;
using Niconicome.Models.Local.Addon.API.Local.Resource;
using Niconicome.Models.Local.Addon.API.Local.Storage;
using Niconicome.Models.Local.Addon.API.Local.Tab;
using Niconicome.Models.Local.Addon.API.Net.Hooks;
using Niconicome.Models.Local.Addon.API.Net.Http.Fetch;
using Niconicome.Models.Domain.Local.Addons.Core.V2.Engine.Infomation;

namespace Niconicome.Models.Local.Addon.API
{
    public interface IAPIEntryPoint : IDisposable
    {
        /// <summary>
        /// Output API
        /// </summary>
        IOutput? output { get; }

        /// <summary>
        /// Hooks API
        /// </summary>
        IHooks? hooks { get; }

        /// <summary>
        /// Log API
        /// </summary>
        ILog? log { get; }

        /// <summary>
        /// Resource API
        /// </summary>
        IPublicResourceHandler? resource { get; }

        /// <summary>
        /// Storage API
        /// </summary>
        IStorage? storage { get; }

        /// <summary>
        /// Tab API
        /// </summary>
        ITabsManager? tab { get; }

        /// <summary>
        /// APIを初期化
        /// </summary>
        /// <param name="infomation"></param>
        void Initialize(IAddonInfomation infomation);
    }

    public class APIEntryPoint : IAPIEntryPoint
    {
        public APIEntryPoint(IOutput output, IHooks hooks, ILog log, IPublicResourceHandler resourceHandler, IStorage storage,ITabsManager tab)
        {
            this.output = output;
            this.hooks = hooks;
            this.log = log;
            this.resource = resourceHandler;
            this.storage = storage;
            this.tab = tab;
        }

        #region Props

        public IOutput? output { get; private set; }

        public IHooks? hooks { get; private set; }

        public ILog? log { get; private set; }

        public IPublicResourceHandler? resource { get; private set; }

        public IStorage? storage { get; private set; }

        public ITabsManager? tab { get; private set; }


        #endregion

        #region methods

        public void Initialize(IAddonInfomation infomation)
        {
            if (!infomation.HasPermission(Permissions.Output))
            {
                this.output = null;
            }
            else
            {
                this.output!.SetInfo(infomation);
            }

            if (!infomation.HasPermission(Permissions.Hooks))
            {
                this.hooks = null;
            }

            if (infomation.HasPermission(Permissions.Log))
            {
                this.log!.Initialize(infomation);
            }
            else
            {
                this.log = null;
            }

            if (infomation.HasPermission(Permissions.Resource))
            {
                this.resource!.Initialize(infomation);
            }
            else
            {
                this.resource = null;
            }

            if (infomation.HasPermission(Permissions.Storage))
            {
                this.storage!.localStorage.Initialize(infomation);
            }
            else
            {
                this.storage = null;
            }

            if (infomation.HasPermission(Permissions.Tab))
            {
                this.tab!.SetInfo(infomation);
            }
            else
            {
                this.tab = null;
            }
        }

        public void Dispose()
        {
            this.storage?.Dispose();
        }

        #endregion
    }
}
