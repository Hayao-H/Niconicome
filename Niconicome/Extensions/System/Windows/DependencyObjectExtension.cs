using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niconicome.Extensions.System.Windows
{
    public static class DependencyObjectExtension
    {
        /// <summary>
        /// 指定された型の親コントロールを取得する
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dependency"></param>
        /// <returns></returns>
        public static T? GetParent<T>(this DependencyObject dependency)
        {
            var parent = VisualTreeHelper.GetParent(dependency);
            return parent switch
            {
                null => default,
                T obj => obj,
                _ => GetParent<T>(parent)
            };
        }

        /// <summary>
        /// 指定した型の子アイテムを取得する
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dependency"></param>
        /// <returns></returns>
        public static List<T> GetChildrenList<T>(this DependencyObject dependency, List<T> children) where T:Visual
        {
            int childCount = VisualTreeHelper.GetChildrenCount(dependency);
            foreach (var i in Enumerable.Range(0, childCount))
            {
                DependencyObject child = VisualTreeHelper.GetChild(dependency, i);
                if (child is T ret)
                {
                    children.Add(ret);
                } else if (child is not null)
                {
                    child.GetChildrenList<T>(children);
                }
            }

            return children;
        }

        /// <summary>
        /// 指定した型の子アイテムを取得する
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dependency"></param>
        /// <returns></returns>
        public static List<T> GetChildrenList<T>(this DependencyObject dependency) where T : Visual
        {
            var children = new List<T>();
            dependency.GetChildrenList<T>(children);
            return children;
        }
    }
}
