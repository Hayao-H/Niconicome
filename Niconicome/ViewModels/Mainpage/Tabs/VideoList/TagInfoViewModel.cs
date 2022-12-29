using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Playlist;

namespace Niconicome.ViewModels.Mainpage.Tabs.VideoList
{
    public class TagInfoViewModel
    {
        public TagInfoViewModel(ITagInfo tag)
        {
            this._tag = tag;
        }

        #region field

        private readonly ITagInfo _tag;

        #endregion

        #region Props

        /// <summary>
        /// ID
        /// </summary>
        public int ID => this._tag.ID;

        /// <summary>
        /// タグ名
        /// </summary>

        public string Name =>this._tag.Name;

        /// <summary>
        /// 大百科記事存在フラウ
        /// </summary>
        public bool IsNicodicExistthis => this._tag.IsNicodicExist;

        #endregion
    }
}
