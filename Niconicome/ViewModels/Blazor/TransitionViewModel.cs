using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WS = Niconicome.Workspaces;

namespace Niconicome.ViewModels.Blazor
{
    public class TransitionViewModel
    {
        public string PageToNavigate => WS::AddonPage.BlazorPageManager.PageToNavigate;
    }
}
