using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Local.Addons.Core;

namespace Niconicome.Models.Local.Addon.API
{
    public interface IAPIBase
    {
        void SetInfo(AddonInfomation infomation);
    }

    public class APIBase : IAPIBase
    {
        public void SetInfo(AddonInfomation infomation)
        {
            this._addonInfomation = infomation;
        }


        protected AddonInfomation? _addonInfomation;
    }
}
