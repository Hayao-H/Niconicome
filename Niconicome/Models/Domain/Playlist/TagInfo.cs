using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Local.Store.V2;

namespace Niconicome.Models.Domain.Playlist
{
    public interface ITagInfo
    {
        public int ID { get; }

        public string Name { get; }

        public bool IsNicodicExist { get; set; }
    }

    public class TagInfo : UpdatableInfoBase<ITagStore, ITagInfo>, ITagInfo
    {
        public TagInfo(ITagStore tagStore) : base(tagStore) { }

        #region field

        private bool _isNicodicExist;

        #endregion

        #region Props

        public int ID { get; init; }

        public string Name { get; init; } = string.Empty;

        public bool IsNicodicExist
        {
            get => this._isNicodicExist;
            set
            {
                this._isNicodicExist = value;
                this.Update(this);
            }
        }

        #endregion
    }
}
