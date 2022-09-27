using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Local.Addons.Core.V2.Permisson;

namespace Niconicome.Models.Domain.Local.Addons.Core.V2.Engine.Infomation
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
        /// ユニークな識別子
        /// </summary>
        string Identifier { get; }

        /// <summary>
        /// アイコンのパス
        /// </summary>
        string IconPath { get; }

        /// <summary>
        /// 説明
        /// </summary>
        string Description { get; }

        /// <summary>
        /// フォルダー名
        /// </summary>
        string DirectoryName { get; }

        /// <summary>
        /// 権限一覧
        /// </summary>
        IReadOnlyList<Permission> Permissions { get; }

        /// <summary>
        /// ホスト権限一覧
        /// </summary>
        IReadOnlyList<string> HostPermissions { get; }

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

        /// <summary>
        /// 更新情報ファイルのURL
        /// </summary>
        string UpdateJsonURL { get; }

        /// <summary>
        /// 自動更新フラグ
        /// </summary>
        bool AutoUpdate { get; }

        /// <summary>
        /// バージョン
        /// </summary>
        Version Version { get; }

        /// <summary>
        /// APIバージョン
        /// </summary>
        Version TargetAPIVersion { get; }

        /// <summary>
        /// 権限を持っているかどうか確認する
        /// </summary>
        /// <param name="permission"></param>
        /// <returns></returns>
        bool HasPermission(Permission permission);

    }

    public class AddonInfomation : IAddonInfomation
    {
        public string ID { get; init; } = Guid.NewGuid().ToString("D");

        public string Name { get; init; } = "";

        public string Identifier { get; init; } = "";


        public string IconPath { get; init; } = "";

        public string Description { get; init; } = "";

        public IReadOnlyList<Permission> Permissions { get; init; } = new List<Permission>();

        public IReadOnlyList<string> HostPermissions { get; init; } = new List<string>();

        public string Author { get; init; } = "";

        public string HomePage { get; init; } = "";

        public string ScriptPath { get; init; } = "";

        public string DllPath { get; init; } = "";

        public string DirectoryName { get; init; } = "";

        public string UpdateJsonURL { get; init; } = "";

        public bool AutoUpdate { get; init; }

        public Version Version { get; init; } = new();

        public Version TargetAPIVersion { get; init; } = new();

        public bool HasPermission(Permission permission) => this.Permissions.Any(p => p == permission);

    }
}
