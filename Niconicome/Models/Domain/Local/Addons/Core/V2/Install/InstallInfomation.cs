using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Local.Addons.Core.V2.Engine.Infomation;

namespace Niconicome.Models.Domain.Local.Addons.Core.V2.Install
{
    public record InstallInfomation(string ManifestPath, string DirectoryPath);
}
