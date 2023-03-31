using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using WS = Niconicome.Workspaces;

namespace Niconicome.ViewModels.Blazor
{
    public class BlazorBaseViewModel
    {
        #region Method

        public void OnKeyDown(NavigationManager navigation)
        {
            WS::Mainpage.BlazorPageManager.ReloadRequested(navigation.Uri);
        }

        #endregion
    }
}
