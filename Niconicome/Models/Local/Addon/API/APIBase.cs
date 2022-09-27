using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Local.Addons.Core.V2.Engine.Infomation;

namespace Niconicome.Models.Local.Addon.API
{
    public interface IAPIBase
    {
        void SetInfo(IAddonInfomation infomation);
    }

    public class APIBase : IAPIBase
    {
        public void SetInfo(IAddonInfomation infomation)
        {
            this._addonInfomation = infomation;
        }


        protected IAddonInfomation? _addonInfomation;
    }
}
