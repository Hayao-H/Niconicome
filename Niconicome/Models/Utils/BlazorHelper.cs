using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace Niconicome.Models.Utils
{
    public interface IBlazorHelper
    {
        /// <summary>
        /// 文字列をHTMLエンコード後、改行をHTMLのbrタグに変換
        /// </summary>
        /// <param name="rawText"></param>
        /// <returns></returns>
        string ConvertNewLine(string rawText);
    }

    public class BlazorHelper : IBlazorHelper
    {
        #region Method

        public string ConvertNewLine(string rawText)
        {
            return Regex.Replace(HttpUtility.HtmlEncode(rawText), Environment.NewLine, "<br>");
        }

        #endregion
    }
}
