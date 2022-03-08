using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ClearScript;

namespace Niconicome.Models.Local.Addon.API
{
    public interface IEventDispatcher
    {
        void addEventListner(string eventName, ScriptObject listener);
    }
}
