using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Extensions.System.List;
using Niconicome.Models.Domain.Utils.StringHandler;

namespace Niconicome.Models.Local.State.Tab.V1
{
    public interface ITabControler
    {
        /// <summary>
        /// タブを開く
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        ITab Open(TabType type);

        /// <summary>
        /// 開いているタブ
        /// </summary>
        ReadOnlyObservableCollection<ITab> Tabs { get; }
    }

    public class TabControler : ITabControler
    {
        public TabControler(IStringHandler stringHandler)
        {
            this._stringHandler = stringHandler;
            this.Open(TabType.Main);
        }

        private readonly ObservableCollection<ITab> tabs = new();

        private readonly IStringHandler _stringHandler;

        public ITab Open(TabType type)
        {
            string id = Guid.NewGuid().ToString();
            ITab tab = new Tab(this._stringHandler.GetContent(type), type, () => this.tabs.RemoveAll(t => t.Type != TabType.Main && t.ID == id), id);
            this.tabs.Add(tab);
            return tab;
        }

        public ReadOnlyObservableCollection<ITab> Tabs => new(this.tabs);
    }

    public enum TabType
    {
        [StringEnum("動画リスト")]
        Main,
        [StringEnum("設定")]
        Settings,
        [StringEnum("ダウンロード")]
        Download,
        [StringEnum("アドオン")]
        Addon
    }
}
