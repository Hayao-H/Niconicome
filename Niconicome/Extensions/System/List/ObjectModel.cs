﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niconicome.Extensions.System.List
{
    static class ObjectModel
    {
        public static void Addrange<T>(this ObservableCollection<T> list,IEnumerable<T>? sources)
        {
            if (sources is null) return;
            foreach (var source in sources)
            {
                list.Add(source);
            }
        }

        /// <summary>
        /// FindIndexの代替。発見できなかった場合は-1を返す。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static int FindIndex<T>(this ObservableCollection<T> list,Predicate<T> predicate)
        {
            foreach (var elm in list.Select((content, index) =>new { index,content}))
            {
                if (predicate(elm.content)) return elm.index;
            }

            return -1;
        }
    }
}
