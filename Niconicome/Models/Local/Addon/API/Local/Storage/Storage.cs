using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niconicome.Models.Local.Addon.API.Local.Storage
{
    public interface IStorage
    {
        ILocalStorage localStorage { get; }
    }

    public class Storage : IStorage
    {
        public Storage(ILocalStorage localStorage)
        {
            this.localStorage = localStorage;
        }

        public ILocalStorage localStorage { get; init; }
    }
}
