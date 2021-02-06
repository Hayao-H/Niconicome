using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Html.Parser;
using AngleSharp.Html.Dom;

namespace Niconicome.Models.Domain.Niconico.Net.Html
{
    public class HtmlParser
    {
        public static IHtmlDocument ParseDocument(string source)
        {
            var config = Configuration.Default;
            var context = BrowsingContext.New(config);
            var parser = context.GetService<IHtmlParser>();
            return parser.ParseDocument(source);
        }
    }
}
