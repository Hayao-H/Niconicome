using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Extensions.System.List;

namespace Niconicome.Models.Domain.Local.Addons.Core
{
    public interface IAddonInfomationsContainer
    {
        AddonInfomation GetAddon(int id);
        void Remove(int id);
        ObservableCollection<AddonInfomation> Addons { get; }
    }

    public class AddonInfomationsContainer : IAddonInfomationsContainer
    {
        #region field

        private readonly Dictionary<int, AddonInfomation> addons = new();

        #endregion

        public ObservableCollection<AddonInfomation> Addons { get; init; } = new();


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
                this.Addons.Add(newData);
                return newData;
            }
        }

        /// <summary>
        /// アドオンを削除
        /// </summary>
        /// <param name="id"></param>
        public void Remove(int id)
        {
            AddonInfomation addon = this.addons[id];
            this.Addons.Remove(addon);
            this.addons.Remove(id);
        }

    }
}
