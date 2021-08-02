using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ClearScript.V8;

namespace Niconicome.Models.Domain.Local.Addons.Core
{
    public interface IJavaScriptExecuter : IDisposable
    {
        void AddHostObject(string name, object obj);
        void AddHostType(string name, Type type);
        void Execute(string code);
        dynamic Evaluate(string code);
        T EvaluateAs<T>(string code);
        dynamic Script { get; }

        /// <summary>
        /// 外部から設定する
        /// </summary>
        /// <param name="flags"></param>
        void Configure(V8ScriptEngineFlags flags);
    }

    public class JavaScriptExecuter : IJavaScriptExecuter
    {
        public JavaScriptExecuter()
        {
            this.engine = new V8ScriptEngine(DefaultFlags);
        }

        ~JavaScriptExecuter()
        {
            this.Dispose();
        }

        #region field

        private V8ScriptEngine engine;

        #endregion

        #region Props

        public dynamic Script { get => this.engine.Script; }

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
        /// 実行する
        /// </summary>
        /// <param name="code"></param>
        public void Execute(string code)
        {
            this.engine.Execute(code);
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

        /// <summary>
        /// 型を追加する
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        public void AddHostType(string name, Type type)
        {
            this.engine.AddHostType(name, type);
        }


        /// <summary>
        /// エンジンを停止する
        /// </summary>
        public void Dispose()
        {
            this.engine.Dispose();
            GC.SuppressFinalize(this);
        }

        public void Configure(V8ScriptEngineFlags flags)
        {
            this.engine = new V8ScriptEngine(flags);
        }

        /// <summary>
        /// デフォルト設定
        /// </summary>
        public static V8ScriptEngineFlags DefaultFlags => V8ScriptEngineFlags.EnableTaskPromiseConversion | V8ScriptEngineFlags.EnableDateTimeConversion;


    }
}
