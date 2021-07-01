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
    }

    public class PermissionsHandler : IPermissionsHandler
    {
        #region

        private List<string>? permissions;

        #endregion

        /// <summary>
        /// 既知の権限であるかどうかをチェックする
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool IsKnownPermission(string name)
        {
            if (this.permissions is null)
            {
                this.permissions = typeof(PermissionNames).GetProperties().Where(p => p.MemberType == MemberTypes.Property).Select(m => (string)(m.GetValue(null) ?? string.Empty)).ToList();
            }

            if (this.permissions.Contains(name))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
