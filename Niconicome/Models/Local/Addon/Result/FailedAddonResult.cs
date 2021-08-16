using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niconicome.Models.Local.Addon.Result
{
    public record FailedAddonResult(string PackageID, string Message, bool CanUninstall);
}
