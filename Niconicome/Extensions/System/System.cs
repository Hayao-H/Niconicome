using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Threading.Tasks;

namespace Niconicome.Extensions.System
{
    public static class StringExtensions
    {
        /// <summary>
        /// 文字列を指定回数繰り返す
        /// </summary>
        /// <param name="source">元の文字列</param>
        /// <param name="count">繰り返す回数</param>
        /// <returns></returns>
        public static string Repeat(this string source,int count)
        {
            var result = source;
            foreach (var i in Enumerable.Range(1,count))
            {
                result += source;
            }
            return result;
        }

        /// <summary>
        /// 文字列を繰り返す
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string Repeat(this string source)
        {
            return StringExtensions.Repeat(source, 2);
        }

        /// <summary>
        /// 文字列がnullまたは空白であるかどうかを判断する
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty([NotNullWhen(false)]this string? source)
        {
            return string.IsNullOrEmpty(source);
        }
    }

    public static class BoolExtensions
    {
        //boolの値を逆にする
        public static bool Not(this bool source)
        {
            return !source;
        }
    }
}
