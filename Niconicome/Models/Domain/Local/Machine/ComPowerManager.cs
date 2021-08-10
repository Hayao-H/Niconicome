using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niconicome.Models.Domain.Local.Machine
{
    public interface IComPowerManager
    {
        /// <summary>
        /// シャットダウン
        /// </summary>
        void Shutdown();

        /// <summary>
        /// 再起動
        /// </summary>
        void Restart();

        /// <summary>
        /// ログオフ
        /// </summary>
        void LogOff();

        /// <summary>
        /// 休止状態
        /// </summary>
        void Sleep();
    }

    public class ComPowerManager : IComPowerManager
    {
        #region Props
        public void Shutdown()
        {
            var p = new Process();
            p.StartInfo.UseShellExecute = true;
            p.StartInfo.FileName = "shutdown.exe";
            p.StartInfo.Arguments = @"/p";

            p.Start();
        }

        public void Restart()
        {

            var p = new Process();
            p.StartInfo.UseShellExecute = true;
            p.StartInfo.FileName = "shutdown.exe";
            p.StartInfo.Arguments = @"/r /f";

            p.Start();
        }

        public void LogOff()
        {

            var p = new Process();
            p.StartInfo.UseShellExecute = true;
            p.StartInfo.FileName = "shutdown.exe";
            p.StartInfo.Arguments = @"/l /f";

            p.Start();
        }

        public void Sleep()
        {

            var p = new Process();
            p.StartInfo.UseShellExecute = true;
            p.StartInfo.FileName = "shutdown.exe";
            p.StartInfo.Arguments = @"/h /f";

            p.Start();
        }
        #endregion
    }
}
