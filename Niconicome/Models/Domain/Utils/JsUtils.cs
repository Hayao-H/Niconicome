using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ClearScript;
using Microsoft.ClearScript.V8;
using Microsoft.Extensions.DependencyInjection;
using Niconicome.Models.Domain.Local.Addons.Core;
using Niconicome.Models.Domain.Local.Addons.Core.V2.Engine.JavaScript;

namespace Niconicome.Models.Domain.Utils
{
    public static class JsUtils
    {
        private static readonly IJavaScriptEngine executer = DIFactory.Provider.GetRequiredService<IJavaScriptEngine>();

        private static bool isInitialized;

        private static void InitializeIfNotInitialized()
        {
            if (isInitialized) return;

            var code = @"
                function toClrArray(jsArray){
                    let clrArray = host.newArr(jsArray.length);
                    for (let i=0;i<jsArray.length;i++){
                        clrArray[i]=jsArray[i];
                    }
                    return clrArray;
                }
            ";

            executer.AddHostObject("host", new HostFunctions());
            executer.Execute(code);
            isInitialized = true;
        }

        /// <summary>
        /// Javascriptの配列を.NETのリストに変換する
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static List<T> ToClrArray<T>(ScriptObject obj)
        {
            InitializeIfNotInitialized();
            dynamic result = executer.Script.toClrArray(obj);
            var list = new List<T>();
            foreach (var item in result)
            {
                if (item is T casted)
                {
                    list.Add(casted);
                }
            }
            return list;
        }

        /// <summary>
        /// <see cref="V8ScriptEngine">V8ScriptEngine</see>から返されるDateTimeタイムゾーンがUTCであるインスタンスを、現地時間に変換する
        /// </summary>
        /// <param name="rawDatetime"></param>
        /// <returns></returns>
        public static DateTime ToLocalDateTime(DateTime rawDatetime)
        {
            TimeZoneInfo tz = TimeZoneInfo.Local;
            DateTime dt = TimeZoneInfo.ConvertTimeFromUtc(rawDatetime, tz);
            return dt;
        }
    }
}
