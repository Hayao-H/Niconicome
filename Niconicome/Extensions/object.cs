using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niconicome.Extensions
{
    static class ObjectExtension
    {
        /// <summary>
        /// 後付け型キャスト
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T As<T>(this object obj) where T : class
        {
            if (obj == null) throw new ArgumentNullException($"nullは{typeof(T)}型にキャストできません。");
            return (obj as T) switch
            {
                T casted => casted,
                null => throw new ArgumentException($"{obj.GetType().Name}は{typeof(T)}型にキャストできません。"),
            };
        }

        /// <summary>
        /// nullable後付け型キャスト
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T? AsNullable<T>(this object obj) where T : class
        {
            return obj as T;
        }

        /// <summary>
        /// 変数を破棄する
        /// </summary>
        /// <param name="obj"></param>
        public static void Void(this object obj)
        {
            obj.ToString();
        }
    }
}
