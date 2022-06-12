using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Local.Addons.Core.V2.Permisson;

namespace Niconicome.Models.Domain.Local.Addons.Core.V2.Engne
{
    public interface IAddonInfomation
    {
        /// <summary>
        /// アドオンの一意なID（起動時に割り当て）
        /// </summary>
        string ID { get; }

        /// <summary>
        /// アドオン名
        /// </summary>
        string Name { get; }

        /// <summary>
        /// アイコンのパス
        /// </summary>
        string IconPath { get; }

        /// <summary>
        /// 説明
        /// </summary>
        string Description { get; }

        /// <summary>
        /// 権限一覧
        /// </summary>
        IReadOnlyList<Permission> Permissions { get; }

        /// <summary>
        /// アドオンの作者名
        /// </summary>
        string Author { get; }

        /// <summary>
        /// アドオンのホームページ
        /// </summary>
        string HomePage { get; }

        /// <summary>
        /// JavaScriptファイルのパス
        /// </summary>
        string ScriptPath { get; }

        /// <summary>
        /// .NET DLLのパス
        /// </summary>
        string DllPath { get; }
    }

    public class AddonInfomation : IAddonInfomation
    {
        public string ID { get; init; } = "";

        public string Name { get; init; } = "";

        public string IconPath { get; init; } = "";

        public string Description { get; init; } = "";

        public IReadOnlyList<Permission> Permissions { get; init; } = new List<Permission>();

        public string Author { get; init; } = "";

        public string HomePage { get; init; } = "";

        public string ScriptPath { get; init; } = "";

        public string DllPath { get; init; } = "";
    }
}
