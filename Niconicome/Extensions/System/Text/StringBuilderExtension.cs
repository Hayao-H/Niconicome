using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Niconicome.Extensions.System.Text
{
    public static class StringBuilderExtension
    {
        public static StringBuilder AddQuery(this StringBuilder builder, NameValueCollection query)
        {
            foreach (var key in query.AllKeys.Select((Value, Index) => new { Value, Index }))
            {
                if (key.Index == 0)
                {
                    builder.Append("?");
                }
                else
                {
                    builder.Append("&");
                }
                builder.Append($"{key.Value}={HttpUtility.UrlEncode(query[key.Value])}");
            }

            return builder;
        }
    }
}
