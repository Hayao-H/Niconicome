using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ClearScript.V8;

namespace Niconicome.Models.Domain.Local.Addons.Core.V2.Engne
{
    public static class JavaScriptEngineFlags
    {
        public static V8ScriptEngineFlags Default { get; private set; } = V8ScriptEngineFlags.EnableTaskPromiseConversion | V8ScriptEngineFlags.EnableDateTimeConversion;

        public static V8ScriptEngineFlags DebugMode { get; private set; } = V8ScriptEngineFlags.EnableTaskPromiseConversion | V8ScriptEngineFlags.EnableDateTimeConversion | V8ScriptEngineFlags.EnableDebugging | V8ScriptEngineFlags.AwaitDebuggerAndPauseOnStart;
    }
}
