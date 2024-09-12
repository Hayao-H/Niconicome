using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Niconicome.Extensions;

namespace Niconicome.Models.Domain.Utils
{
    static class Utils
    {
        /// <summary>
        /// カーソルの下にある要素を取得する
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="itemsControl"></param>
        /// <param name="getPosition"></param>
        /// <returns></returns>
        public static T? HitTest<T>(UIElement itemsControl, Point pos) where T : class
        {
            var result = itemsControl.InputHitTest(pos).As<DependencyObject>();
            return result switch
            {
                T elm => elm,
                null => null,
                _ => null
            };
        }

        /// <summary>
        /// 文字列をBrushに変換する
        /// </summary>
        /// <param name="colorString"></param>
        /// <returns></returns>
        public static Brush ConvertToBrush(string colorString)
        {
            var color = ColorConverter.ConvertFromString(colorString);
            return new SolidColorBrush((Color)color);
        }

        /// <summary>
        /// 指定長のランダムな文字列を取得する
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string GetRandomString(int length)
        {
            var random = new Random();
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static string GetStrongRandomString(int length)
        {
            var random = new Random();
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!#$%&'()*+,-./:;<=>?@[]^_`{|}~";
            return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }

}
