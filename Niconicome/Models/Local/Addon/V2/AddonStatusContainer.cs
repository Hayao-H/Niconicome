using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Extensions.System.List;
using Niconicome.Models.Domain.Local.Addons.Core.V2.Engine.Infomation;
using Niconicome.Models.Domain.Local.Addons.Core.V2.Loader;

namespace Niconicome.Models.Local.Addon.V2
{

    public interface IAddonStatusContainer
    {
        /// <summary>
        /// 実行中のアドオン
        /// </summary>
        ObservableCollection<IAddonInfomation> LoadedAddons { get; }

        /// <summary>
        /// 読み込みに失敗したアドオン
        /// </summary>
        ObservableCollection<FailedResult> LoadFailedAddons { get; }

        /// <summary>
        /// アップデートが存在するアドオン
        /// </summary>
        ObservableCollection<IAddonInfomation> ToBeUpdatedAddons { get; }

        /// <summary>
        /// 指定されたIDを持つアドオンの情報を削除
        /// </summary>
        /// <param name="ID"></param>
        void Remove(string ID);
    }

    public class AddonStatusContainer : IAddonStatusContainer
    {

        public ObservableCollection<IAddonInfomation> LoadedAddons { get; init; } = new();


        public ObservableCollection<FailedResult> LoadFailedAddons { get; init; } = new();

        public ObservableCollection<IAddonInfomation> ToBeUpdatedAddons { get; init; } = new();

        public void Remove(string ID)
        {
            this.LoadedAddons.RemoveAll(i => i.ID == ID);
            this.ToBeUpdatedAddons.RemoveAll(i => i.ID == ID);
        }


    }
}
