using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Local.Addons.API.Storage.LocalStorage;
using Niconicome.Models.Domain.Local.Addons.Core.Engine;
using Niconicome.Models.Helper.Result;
using NiconicomeTest.Stabs.Models.Domain.Local.IO;
using NiconicomeTest.Stabs.Models.Domain.Utils;

namespace NiconicomeTest.Stabs.Models.Domain.Local.Addon.API.Storage
{
    class StorageHelperStab : StorageHelper, IStorageHelper
    {
        public StorageHelperStab() : base(new NicoFileIOMock(() => true, () => ""), new AddonLogger(new LoggerStab()))
        {

        }

        public new IAttemptResult<StorageData> LoadFile(string packageID)
        {
            return new AttemptResult<StorageData>() { IsSucceeded = true, Data = new StorageData() { PackageID = "" } };
        }

        public new IAttemptResult SaveFile(string packageID, StorageData store)
        {
            return new AttemptResult() { IsSucceeded = true };
        }
    }
}
