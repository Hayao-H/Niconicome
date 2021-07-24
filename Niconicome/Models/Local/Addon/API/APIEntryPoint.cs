using System;
using System.Collections.Generic;
using System.Linq;
using System.Printing;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Local.Addons.Core;
using Niconicome.Models.Domain.Local.Addons.Core.Permisson;

namespace Niconicome.Models.Local.Addon.API
{
    public interface IAPIEntryPoint
    {
        /// <summary>
        /// 出力
        /// </summary>
        IOutput? output { get; }

        void Initialize(AddonInfomation infomation);
    }

    public class APIEntryPoint : IAPIEntryPoint
    {
        public APIEntryPoint(IOutput output)
        {
            this.output = output;
        }

        #region Props

        public IOutput? output { get; private set; }

        #endregion

        #region methods

        public void Initialize(AddonInfomation infomation)
        {
            List<string> permissions = infomation.Permissions;
            if (!permissions.Contains(PermissionNames.Output))
            {
                this.output = null;
            } else
            {
                this.output!.SetInfo(infomation);
            }
        }


        #endregion
    }
}
