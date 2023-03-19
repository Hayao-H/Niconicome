using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Utils.StringHandler;

namespace Niconicome.Models.Domain.Local.External.Import.Niconicome.StringContent
{
    public enum ExportSC
    {
        [StringEnum("niconicome-export-{0}.json")]
        FileName,
    }
}
