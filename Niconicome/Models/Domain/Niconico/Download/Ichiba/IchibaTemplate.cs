using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Const;
using Niconicome.Models.Domain.Niconico.Video.Ichiba;
using Const = Niconicome.Models.Const;

namespace Niconicome.Models.Domain.Niconico.Download.Ichiba
{
    static class IchibaItemTemplate
    {
        public static string GetReplacedString(IIchibaItem item)
        {
            return IchibaItemTemplate.template
                .Replace("{thumb_url}", item.ThumbUrl)
                .Replace("{item_name}", item.Name)
                .Replace("{item_price}", item.Price)
                .Replace("{item_category}", item.Category)
                .Replace("{item_url}", item.LinkUrl);
        }

        private const string template = "<li class=\"market_item\"><div class=\"market_item_img_wrapper\"><a target=\"_blank\" href = \"{thumb_url}\" ><img src=\"{thumb_url}\"></a><span class=\"img_tip\">ニコニコ市場</span></div><div class=\"market_item_info_wrapper\"><p class=\"market_item_name\" title=\"{item_name}\">{item_name}</p><p>{item_price}</p><p>{item_category}</p><a href=\"{item_url}\" target=\"_blank\">売り場に行く</a></div></li>";
    }

    public static class IchibaTemplate
    {
        public static string GetReplacedString(string niconicoID)
        {
            return IchibaTemplate.template
                .Replace("{title}", niconicoID + "の市場情報")
                .Replace("{watch_url}", Const::Net.NiconicoShortUrl + niconicoID);
        }

        private readonly static string template = "<!DOCTYPE html><html><head><title>{title}</title><meta charset=\"utf8\" /><style> p{ font-size: 18px; }" + Environment.NewLine +
            ":link{ color: #3794d1; } " + Environment.NewLine +
            "#title { display: inline; background: linear-gradient(transparent 70%, #3794d1 50%); }" + Environment.NewLine +
            " #title a{ color: black; text-decoration: none;} " + Environment.NewLine +
            "#market_items_container{ display: flex; flex-direction: row; flex-wrap: wrap; } " + Environment.NewLine +
            ".market_item{ height: 350px; width: 200px; list-style: none; margin: 10px; background-color: #e7e7e7; padding: 5px; } " + Environment.NewLine +
            ".market_item p{ margin: 2px 0; text-overflow: ellipsis; white-space: nowrap; overflow: hidden; } " + Environment.NewLine +
            ".market_item_img_wrapper{ height: 60%; width: 100%; background-color: #000; text-align: center; position: relative; } " + Environment.NewLine +
            ".market_item_img_wrapper img { height: 100%; max-width: 100%;} " + Environment.NewLine +
            ".market_item_name{ font-weight: 700; display: inline; } " + Environment.NewLine +
            ".market_item_info_wrapper{ display: flex; flex-direction: column; text-align: center; } " + Environment.NewLine +
            "#footer{ text-align: center; background-color: rgb(34, 34, 34); color: whitesmoke;  padding: 8px; } " + Environment.NewLine +
            ".img_tip{ position: absolute; background-color: rgba(0, 0, 0, 0.575); bottom: 3px; right: 5px; color: white;} " + Environment.NewLine +
            "</style></head><body><div><h1 id=\"title\"><a target=\"_blank\" href=\"{watch_url}\">{title}</a></h1></div><ul id=\"market_items_container\">{items}</ul></body><footer id=\"footer\"><p>This document was produced automatically by <a href=\"https://github.com/Hayao-H/Niconicome\">Niconicome</a>.</p></footer></html>";
    }
}
