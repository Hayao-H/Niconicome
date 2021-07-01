using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niconicome.Models.Domain.Local.Addons.Core.Permisson
{
    public static class Permissions
    {
        public static Permission Hook { get; private set; } = new(PermissionNames.Hook, "Hooks API。この権限を持つ拡張機能はアプリの処理を代替することができます。");
    }

    public static class PermissionNames
    {
        public static string Hook { get; private set; } = "hook";
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
