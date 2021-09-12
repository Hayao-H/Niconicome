using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Composition;

namespace Niconicome.Models.Domain.Local.Addons.API.Storage.LocalStorage
{
    public class StorageData
    {

        /// <summary>
        /// パッケージID
        /// </summary>
        public string PackageID { get; set; } = string.Empty;

        /// <summary>
        /// データの実体
        /// </summary>
        public Dictionary<string, string> Data { get; set; } = new();
    }
}
