using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ClearScript;
using Niconicome.Models.Domain.Local.Addons.API.Hooks;

namespace Niconicome.Models.Local.Addon.API.Net.Hooks
{
    public interface IHooks
    {
        /// <summary>
        /// ページ解析関数を登録する
        /// </summary>
        /// <param name="function"></param>
        void registerPageAnalyzeFunction(ScriptObject function);

        /// <summary>
        /// セッション確立関数を登録する
        /// </summary>
        /// <param name="function"></param>
        void registerSessionEnsuringFunction(ScriptObject function);
    }

    class Hooks : IHooks
    {
        public Hooks(IHooksManager manager)
        {
            this.manager = manager;
        }

        #region field

        private readonly IHooksManager manager;

        #endregion

        #region Methods

        public void registerPageAnalyzeFunction(ScriptObject function)
        {
            this.manager.Register(function, HookType.WatchPageParser);
        }

        public void registerSessionEnsuringFunction(ScriptObject function)
        {
            this.manager.Register(function, HookType.SessionEnsuring);
        }


        #endregion
    }
}
