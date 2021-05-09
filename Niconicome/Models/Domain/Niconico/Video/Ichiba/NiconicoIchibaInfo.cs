using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niconicome.Models.Domain.Niconico.Video.Ichiba
{

    public interface IIchibaItem
    {
        string Name { get; }
        string Category { get; }
        string Price { get; }
        string LinkUrl { get; }
    }

    public interface INiconicoIchibaInfo
    {
        List<IIchibaItem> IchibaItems { get; }
    }

    /// <summary>
    /// 商品情報のラッパー
    /// </summary>
    class NiconicoIchibaInfo: INiconicoIchibaInfo
    {
        public List<IIchibaItem> IchibaItems { get; init; } = new();
    }

    /// <summary>
    /// 商品情報
    /// </summary>
    class IchibaItem : IIchibaItem
    {
        public string Name { get; set; } = string.Empty;

       public string Category { get; set; } = string.Empty;

        public string Price { get; set; } = string.Empty;

        public string LinkUrl { get; set; } = string.Empty;
    }
}
