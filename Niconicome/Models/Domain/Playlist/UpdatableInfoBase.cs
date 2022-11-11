using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Local.Store.V2;

namespace Niconicome.Models.Domain.Playlist
{
    public class UpdatableInfoBase<TUpdater, TInfo> where TUpdater : IStoreUpdater<TInfo>
    {
        public UpdatableInfoBase(IStoreUpdater<TInfo> updater)
        {
            this._updater = updater;
        }


        #region field

        private readonly IStoreUpdater<TInfo> _updater;

        #endregion

        protected void Update(TInfo infomation)
        {
            this._updater.Update(infomation);
        }
    }
}
