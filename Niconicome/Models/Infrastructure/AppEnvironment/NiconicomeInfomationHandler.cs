using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Utils.AppEnvironment;

namespace Niconicome.Models.Infrastructure.AppEnvironment
{
    public class NiconicomeInfomationHandler : IAppInfomationHandler
    {

        public Version ApplicationVersion => Assembly.GetExecutingAssembly().GetName().Version ?? new Version("0.0.0");

        public bool Is64BitProcess => Environment.Is64BitProcess;
    }
}
