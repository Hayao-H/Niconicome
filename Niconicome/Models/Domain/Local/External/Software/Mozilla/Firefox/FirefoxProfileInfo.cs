using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niconicome.Models.Domain.Local.External.Software.Mozilla.Firefox
{
    public interface IFirefoxProfileInfo
    {
        string ProfileName { get; }
        string ProfilePath { get; }
        string CookiePath { get; }
    }

    public class FirefoxProfileInfo:IFirefoxProfileInfo
    {
        public string ProfileName { get; set; } = string.Empty;

        public string ProfilePath { get; set; } = string.Empty;

       public string CookiePath { get => Path.Combine(this.ProfilePath, "cookies.sqlite"); }
    }
}
