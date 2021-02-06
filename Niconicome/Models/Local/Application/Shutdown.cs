using System;
using Niconicome.Models.Domain.Local;

namespace Niconicome.Models.Local.Application
{
    public interface IShutdown
    {
        void ShutdownApp();
    }

    public class Shutdown : IShutdown
    {
        public Shutdown(IDataBase dataBase)
        {
            this.dataBase = dataBase;
        }

        private readonly IDataBase dataBase;

        private bool isShutdowned;

        /// <summary>
        /// 最終処理
        /// </summary>
        public void ShutdownApp()
        {
            if (this.isShutdowned) throw new InvalidOperationException("終了処理は一度のみ可能です。");
            this.dataBase.Dispose();
            this.isShutdowned = true;
        }
    }
}
