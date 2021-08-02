using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Microsoft.ClearScript;
using Microsoft.ClearScript.V8;
using Microsoft.Extensions.DependencyInjection;
using Niconicome.Extensions.System;
using Niconicome.Models.Const;
using Niconicome.Models.Domain.Local.IO;
using Niconicome.Models.Domain.Utils;
using Niconicome.Models.Helper.Result;
using Niconicome.Models.Local.Settings;
using Const = Niconicome.Models.Const;

namespace Niconicome.Models.Domain.Local.Addons.Core.Engine.Context
{
    public interface IAddonContext : IDisposable
    {
        AddonInfomation? AddonInfomation { get; }
        bool IsInitialized { get; }
        bool HasError { get; }
        Exception? Exception { get; }
        IAttemptResult Initialize(AddonInfomation infomation, Action<IJavaScriptExecuter> factory, bool isDebuggingEnable);
    }

    public class AddonContext : IAddonContext
    {
        public AddonContext(INicoFileIO fileIO, IAddonLogger addonLogger, IJavaScriptExecuter executer, ILogger logger)
        {
            this.fileIO = fileIO;
            this.addonLogger = addonLogger;
            this.Executer = executer;
            this.logger = logger;
        }

        ~AddonContext()
        {
            this.Dispose();
        }

        /// <summary>
        /// インスタンスを作成
        /// </summary>
        /// <returns></returns>
        public static IAddonContext CreateInstance()
        {
            return DIFactory.Provider.GetRequiredService<IAddonContext>();
        }

        #region field

        private readonly INicoFileIO fileIO;

        private readonly IAddonLogger addonLogger;

        private readonly ILogger logger;

        #endregion

        #region Props
        public IJavaScriptExecuter Executer { get; init; }

        public AddonInfomation? AddonInfomation { get; private set; }

        public bool IsInitialized { get; private set; }

        public bool HasError => this.Exception is not null;

        public Exception? Exception { get; private set; }

        #endregion

        /// <summary>
        /// アドオンを初期化する
        /// </summary>
        /// <param name="infomation"></param>
        /// <returns></returns>
        public IAttemptResult Initialize(AddonInfomation infomation, Action<IJavaScriptExecuter> factory, bool isDebuggingEnable)
        {
            if (this.IsInitialized) return new AttemptResult() { Message = "既に初期化されています。" };

            if (infomation.Scripts.BackgroundScript.IsNullOrEmpty())
            {
                return new AttemptResult() { IsSucceeded = true };
            }

            this.AddonInfomation = infomation;
            string codePath = Path.Combine(FileFolder.AddonsFolder, infomation.PackageID.Value, infomation.Scripts.BackgroundScript);
            string code;

            try
            {
                code = this.fileIO.OpenRead(codePath);
            }
            catch (Exception e)
            {
                this.addonLogger.Error("コードの読み込みに失敗しました。", infomation.Name.Value, e);
                this.Exception = e;
                return new AttemptResult() { Message = "コードの読み込みに失敗しました。", Exception = e };
            }

            if (isDebuggingEnable)
            {
                V8ScriptEngineFlags defaultSettings = JavaScriptExecuter.DefaultFlags;
                defaultSettings = defaultSettings | V8ScriptEngineFlags.EnableDebugging | V8ScriptEngineFlags.AwaitDebuggerAndPauseOnStart;
                this.Executer.Configure(defaultSettings);
            }

            factory(this.Executer);

            this.InitializeInternal();

            _ = Task.Run(() =>
            {
                try
                {
                    this.Executer.Evaluate(code);
                }
                catch (Exception e)
                {
                    this.addonLogger.Error("アドオンの実行に失敗しました。", this.AddonInfomation.Name.Value, e);
                }
            });

            return new AttemptResult() { IsSucceeded = true };

        }

        /// <summary>
        /// インスタンスを破棄する
        /// </summary>
        public void Dispose()
        {
            this.Executer.Dispose();
            GC.SuppressFinalize(this);
        }

        #region private

        private void InitializeInternal()
        {
            Action<ScriptObject, int> setTimeout = (function, delay) =>
             {
                 var timer = new Timer(delay);
                 timer.Elapsed += (_, _) =>
                 {
                     function.Invoke(false);
                 };
                 timer.Enabled = true;
             };
            this.Executer.AddHostObject("setTimeout", setTimeout);
        }

        #endregion


    }
}
