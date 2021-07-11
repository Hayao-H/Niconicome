using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niconicome.Models.Domain.Local.Addons.Core
{
    public interface IAddonInfomationsContainer
    {
        AddonInfomation GetAddon(int id);
    }

    public class AddonInfomationsContainer : IAddonInfomationsContainer
    {
        #region field

        private Dictionary<int, AddonInfomation> addons = new();

        #endregion

        /// <summary>
        /// アドオンを取得する
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public AddonInfomation GetAddon(int id)
        {
            this.addons.TryGetValue(id, out AddonInfomation? addon);

            if (addon is not null)
            {
                return addon;
            }
            else
            {
                var newData = new AddonInfomation();
                newData.ID.Value = id;
                this.addons.Add(id, newData);
                return newData;
            }
        }
    }
}
