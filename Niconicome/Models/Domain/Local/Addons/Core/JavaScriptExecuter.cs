using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ClearScript.V8;

namespace Niconicome.Models.Domain.Local.Addons.Core
{
    public interface IJavaScriptExecuter
    {
        void AddHostObject(string name, object obj);
        dynamic Evaluate(string code);
        T EvaluateAs<T>(string code);
    }

    public class JavaScriptExecuter : IJavaScriptExecuter
    {
        public JavaScriptExecuter()
        {
            this.engine = new V8ScriptEngine(V8ScriptEngineFlags.EnableTaskPromiseConversion);
        }

        #region field

        private readonly V8ScriptEngine engine;

        #endregion

        /// <summary>
        /// JavaScriptのコードを実行する
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public dynamic Evaluate(string code)
        {
            return this.engine.Evaluate(code);
        }

        /// <summary>
        /// 型付きでJavaScriptのコードを実行する
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="code"></param>
        /// <returns></returns>
        public T EvaluateAs<T>(string code)
        {
            return (T)this.Evaluate(code);
        }

        /// <summary>
        /// ホストのアイテムを追加する
        /// </summary>
        /// <param name="name"></param>
        /// <param name="obj"></param>
        public void AddHostObject(string name, object obj)
        {
            this.engine.AddHostObject(name, obj);
        }

    }
}
