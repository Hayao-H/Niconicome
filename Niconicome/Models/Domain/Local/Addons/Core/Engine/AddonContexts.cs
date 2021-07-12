using System.Collections.Generic;

namespace Niconicome.Models.Domain.Local.Addons.Core.Engine
{
    interface IAddonContexts
    {
        Dictionary<int, IJavaScriptExecuter> Contexts { get;}
        void Kill(int id);
        void KillAll();
    }

    class AddonContexts : IAddonContexts
    {

        public Dictionary<int, IJavaScriptExecuter> Contexts { get; init; } = new();

        /// <summary>
        /// 特定のアドオンを停止する
        /// </summary>
        /// <param name="id"></param>
        public void Kill(int id)
        {
            this.Contexts[id].Dispose();
            this.Contexts.Remove(id);
        }

        /// <summary>
        /// すべて停止する
        /// </summary>
        public void KillAll()
        {
            foreach (var addon in this.Contexts)
            {
                addon.Value.Dispose();
            }
        }

    }
}
