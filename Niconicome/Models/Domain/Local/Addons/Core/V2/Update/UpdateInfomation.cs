using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Local.Addons.Core.V2.Engine.Infomation;
using Niconicome.Models.Domain.Local.Addons.Core.V2.Permisson;

namespace Niconicome.Models.Domain.Local.Addons.Core.V2.Update
{
    public record UpdateInfomation(string Version, List<Permission> NewPermissions, bool HasNewPermission, IAddonInfomation AddonInfomation, string ChangeLogURL, string archivePath);
}
