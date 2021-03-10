using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace Niconicome.Models.Domain.Utils
{
    public static class XmlUtils
    {
        /// <summary>
        /// XMl‚Ì§Œä•¶š‚ğæ‚èœ‚­
        /// </summary>
        /// <param name="origin"></param>
        /// <returns></returns>
        public static string RemoveSymbols(string origin)
        {
            string r = "[\x00-\x08\x0B\x0C\x0E-\x1F\x26]";
            return Regex.Replace(origin, r, "", RegexOptions.Compiled);
        }
    }

}