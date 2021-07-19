using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Niconicome.Models.Domain.Local.Addons.Core.Permisson
{
    public interface IPermissionsHandler
    {
        bool IsKnownPermission(string name);

        /// <summary>
        /// 権限情報を取得する
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        Permission? GetPermission(string name);
    }

    public class PermissionsHandler : IPermissionsHandler
    {
        #region

        private List<string>? permissions;

        private List<Permission>? permissionDetails;

        #endregion

        /// <summary>
        /// 既知の権限であるかどうかをチェックする
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool IsKnownPermission(string name)
        {
            this.SetPermissionsIfNull();

            if (this.permissions!.Contains(name))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public Permission? GetPermission(string name)
        {
            this.SetPermissionsIfNull();

            if (!this.IsKnownPermission(name)) return null;

            return this.permissionDetails!.First(p => p.Name == name);

        }

        #region private

        private void SetPermissionsIfNull()
        {
            if (this.permissions == null)
            {
                this.permissions = typeof(PermissionNames).GetProperties().Where(p => p.MemberType == MemberTypes.Property).Select(m => (string)(m.GetValue(null) ?? string.Empty)).ToList();
            }

            if (this.permissionDetails == null)
            {
                this.permissionDetails = typeof(Permissions).GetProperties().Where(p=>p.MemberType == MemberTypes.Property).Select(m => (Permission?)(m.GetValue(null))??new Permission(string.Empty,string.Empty)).Where(p=>!string.IsNullOrEmpty(p.Name)).ToList();
            }
        }

        #endregion

    }
}
