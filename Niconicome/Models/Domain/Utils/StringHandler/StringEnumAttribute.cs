using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niconicome.Models.Domain.Utils.StringHandler
{
    public class StringEnumAttribute : Attribute
    {
        public StringEnumAttribute(string content)
        {
            this.Content = content;
        }

        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; init; }
    }
}
