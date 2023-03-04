using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Utils.StringHandler;

namespace Niconicome.Models.Domain.Niconico.Remote.V2.Search
{
    public enum SearchStringContent
    {
        [StringEnum("検索結果(キーワード):{0}")]
        ResultTitleKw,
        [StringEnum("検索結果(タグ):{0}")]
        ResultTitleTag,
    }
}
