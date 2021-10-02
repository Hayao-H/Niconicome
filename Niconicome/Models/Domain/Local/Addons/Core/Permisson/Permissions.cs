using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Pwm;

namespace Niconicome.Models.Domain.Local.Addons.Core.Permisson
{
    public static class Permissions
    {
        public static Permission Hooks { get; private set; } = new(PermissionNames.Hooks, "Hooks API。この権限を持つ拡張機能はアプリの処理を代替することができます。");

        public static Permission Output { get; private set; } = new(PermissionNames.Output, "Output API。この権限を持つ拡張機能はアプリケーションの出力画面に文字列を書き込むことが出来ます。");

        public static Permission Log { get; private set; } = new(PermissionNames.Log, "Log API。この権限を持つ拡張機能はアプリケーションのログに文字列を書き込むことが出来ます。");

        public static Permission Session { get; private set; } = new(PermissionNames.Session, "Session API。この権限を持つ拡張機能はログイン状態でウェブサイトにリクエストを送信することができます。");

        public static Permission Storage { get; private set; } = new(PermissionNames.Storage, "Storage API。この権限を持つ拡張機能はデータをローカルに永続化することができます。（保存されたデータはアンインストール時に削除されます。）");

        public static Permission Resource { get; private set; } = new(PermissionNames.Resource, "Resource API。この権限を持つ拡張機能は拡張機能フォルダー内のresourceディレクトリのファイルを動的に読み込むことができます。");
    }

    public static class PermissionNames
    {
        public static string Hooks { get; private set; } = "hooks";

        public static string Output { get; private set; } = "output";

        public static string Log { get; private set; } = "log";

        public static string Session { get; private set; } = "session";

        public static string Storage { get; private set; } = "storage";

        public static string Resource { get; private set; } = "resource";
    }

    public class Permission
    {
        public Permission(string name, string description)
        {
            this.Name = name;
            this.Description = description;
        }

        public string Name { get; init; }

        public string Description { get; init; }
    }
}
