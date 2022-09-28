using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    }

    public class AddonStatusContainer : IAddonStatusContainer
    {

        public ObservableCollection<IAddonInfomation> LoadedAddons { get; init; } = new();


        public ObservableCollection<FailedResult> LoadFailedAddons { get; init; } = new();

        public ObservableCollection<IAddonInfomation> ToBeUpdatedAddons { get; init; } = new();

    }
}
