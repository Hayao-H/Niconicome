using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Niconicome.Models.Domain.Utils.StringHandler
{
    public interface IStringHandler
    {
        /// <summary>
        /// 文字列を取得
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        string GetContent<T>(T value) where T : struct, Enum;

        /// <summary>
        /// フォーマットされた文字列を取得
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="items"></param>
        /// <returns></returns>
        string GetContent<T>(T value, params object?[] items) where T : struct, Enum;

    }

    public class StringHandler : IStringHandler
    {
        #region Method

        public string GetContent<T>(T value) where T : struct, Enum => value.GetType().GetField(value.ToString())?.GetCustomAttribute<StringEnumAttribute>()?.Content ?? string.Empty;

        public string GetContent<T>(T value, params object?[] items) where T : struct, Enum
        {
            return string.Format(this.GetContent(value), items);
        }

        #endregion
    }
}
