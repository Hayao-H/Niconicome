using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Niconicome.Models.Domain.Utils.AppEnvironment;

namespace Niconicome.Models.Infrastructure.AppEnvironment
{
    public class WindowsInfomationHandler : IOSInfomationHandler
    {
        public Version OSversion => Environment.OSVersion.Version;

        public string OSversionString
        {
            get
            {
                Version v = this.OSversion;
                PlatformID platform = Environment.OSVersion.Platform;
                string osversion = $"{v.Major}.{v.Minor}";
                int build = v.Build;

                if (platform != PlatformID.Win32NT)
                {
                    return "Not a Windows OS";
                }

                if (osversion == "10.0")
                {
                    if (build >= 22000)
                    {
                        return "Windows 11";
                    }
                    else
                    {
                        return "Windows 10";
                    }
                }
                else if (osversion == "6.3")
                {
                    return "Windows 8.1";
                }
                else if (osversion == "6.2")
                {
                    return "Windows 8";
                }
                else if (osversion == "6.1")
                {
                    return "Windows 7";
                }
                else if (osversion == "6.0")
                {
                    return "Windows Vista";
                }
                else if (osversion == "5.1")
                {
                    return "Windows XP";
                }
                else
                {
                    return "unknown OS (Windows)";
                }
            }
        }
        public bool Is64BitOperatingSYstem => Environment.Is64BitOperatingSystem;

        public double WorkAreWidth => SystemParameters.WorkArea.Width;

        public double WorkAreHeight => SystemParameters.WorkArea.Height;
    }
}
