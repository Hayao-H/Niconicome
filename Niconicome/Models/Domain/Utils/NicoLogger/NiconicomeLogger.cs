using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Local.Settings;
using Niconicome.Models.Helper.Result;

namespace Niconicome.Models.Domain.Utils.NicoLogger
{
    public interface INiconicomeLogger
    {
        /// <summary>
        /// エラー
        /// </summary>
        /// <param name="message"></param>
        void Error(string message);

        /// <summary>
        /// 警告
        /// </summary>
        /// <param name="message"></param>
        void Warning(string message);

        /// <summary>
        /// ログ
        /// </summary>
        /// <param name="message"></param>
        void Log(string message);

        /// <summary>
        /// デバッグフラグ
        /// </summary>
        bool IsDebugMode { get; set; }
    }

    public class NiconicomeLogger : INiconicomeLogger
    {
        public NiconicomeLogger(ILogWriter writer)
        {
            this._writer = writer;
#if DEBUG
            //this.IsDebugMode = true;
#endif
        }

        #region field

        private readonly ILogWriter _writer;

        #endregion

        #region Method

        public void Error(string message)
        {
            this.WriteInternal($"[error]{message}");
        }

        public void Warning(string message)
        {
            this.WriteInternal($"[warning]{message}");
        }

        public void Log(string message)
        {
            if (!this.IsDebugMode) return;
            this.WriteInternal($"[log]{message}");
        }

        #endregion

        #region Props

        public bool IsDebugMode { get; set; }

        #endregion

        #region private

        private void WriteInternal(string source)
        {
            var dt = DateTime.Now.ToString("HH:mm.ss");
            this._writer.Write($"[{dt}]{source}");
        }

        #endregion


    }
}
