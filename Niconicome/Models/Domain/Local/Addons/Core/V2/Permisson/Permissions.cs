﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Pwm;

namespace Niconicome.Models.Domain.Local.Addons.Core.V2.Permisson
{
    public static class Permissions
    {
        public static Permission Hooks { get; private set; } = new(PermissionNames.Hooks, "Hooks API。この権限を持つ拡張機能はアプリの処理を代替することができます。");

        public static Permission Output { get; private set; } = new(PermissionNames.Output, "Output API。この権限を持つ拡張機能はアプリケーションの出力画面に文字列を書き込むことが出来ます。");

        public static Permission Log { get; private set; } = new(PermissionNames.Log, "Log API。この権限を持つ拡張機能はアプリケーションのログに文字列を書き込むことが出来ます。");

        public static Permission Session { get; private set; } = new(PermissionNames.Session, "Session API。この権限を持つ拡張機能はログイン状態でウェブサイトにリクエストを送信することができます。");

        public static Permission Storage { get; private set; } = new(PermissionNames.Storage, "Storage API。この権限を持つ拡張機能はデータをローカルに永続化することができます。（保存されたデータはアンインストール時に削除されます。）");

        public static Permission Resource { get; private set; } = new(PermissionNames.Resource, "Resource API。この権限を持つ拡張機能は拡張機能フォルダー内のresourceディレクトリのファイルを動的に読み込むことができます。");

        public static Permission Tab { get; private set; } = new(PermissionNames.Tab, "Tab API。この権限を持つ拡張機能は画面下部のタブを追加して文書を表示したり、Host権限を持つサイトを表示し、そのサイト上の全てのデータを読み取ったりすることができます。");

        public static Permission DownloadSettings { get; private set; } = new(PermissionNames.DownloadSettings, "Download Settings API。この権限を持つ拡張機能はダウンロード設定の変更、動画のステージング、ダウンロードの実行、ダウンロードのキャンセル及びダウンロード可否の読み取りが可能になります。");

        public static Permission RemoteUpdate { get; private set; } = new(PermissionNames.RemoteUpdate, "Remote Update API。この権限を持つ拡張機能は自身のリモート更新が可能になります。");
    }

    public static class PermissionNames
    {
        public static string Hooks { get; private set; } = "hooks";

        public static string Output { get; private set; } = "output";

        public static string Log { get; private set; } = "log";

        public static string Session { get; private set; } = "session";

        public static string Storage { get; private set; } = "storage";

        public static string Resource { get; private set; } = "resource";

        public static string Tab { get; private set; } = "tab";

        public static string DownloadSettings { get; private set; } = "downloadSettings";

        public static string RemoteUpdate { get; private set; } = "remoteUpdate";
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
