using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niconicome.Models.Local.State.Tab.V1
{
    public interface ITab
    {
        /// <summary>
        /// タブの名前
        /// </summary>
        string Name { get; }

        /// <summary>
        /// タブのID
        /// </summary>
        string ID { get; }

        /// <summary>
        /// タブの種類
        /// </summary>
        TabType Type { get; }

        /// <summary>
        /// タブを閉じる
        /// </summary>
        void Close();
    }

    public class Tab : ITab
    {
        private readonly Action _close;

        public Tab(string name, TabType type, Action close, string iD)
        {
            this.Name = name;
            this._close = close;
            this.ID = iD;
            this.Type = type;
        }

        public string Name { get; init; }

        public string ID { get; init; }

        public TabType Type { get; init; }

        public void Close()
        {
            this._close();
        }
    }
}
