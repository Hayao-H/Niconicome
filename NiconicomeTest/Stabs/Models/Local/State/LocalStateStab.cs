using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Local.Settings;
using Niconicome.Models.Helper.Result;
using Niconicome.Models.Local.State;

namespace NiconicomeTest.Stabs.Models.Local.State
{
    public class LocalStateStub : ILocalState
    {
        public bool IsSettingWindowOpen { get; set; }

        public bool IsDebugMode { get; set; }

        public bool IsImportingFromXeno { get; set; }

        public bool IsAddonManagerOpen { get; set; }

        public bool IsTaskWindowOpen { get; set; }

        public bool IsSettingTabOpen { get; set; }

    }
}
