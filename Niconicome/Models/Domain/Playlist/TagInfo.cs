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
        /// <summary>
        /// ID
        /// </summary>
        public int ID { get; }

        /// <summary>
        /// タグ名
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 大百科記事存在フラウ
        /// </summary>
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
                if (this.IsAutoUpdateEnabled)
                {
                    this.Update(this);
                }
            }
        }

        #endregion
    }
}
