using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Niconicome.Models.Domain.Local.IO;
using Niconicome.Models.Domain.Utils;
using Niconicome.Models.Helper.Result;
using Const = Niconicome.Models.Const;

namespace Niconicome.Models.Domain.Local.Addons.Core.Engine.Context
{
    public interface IAddonContext : IDisposable
    {
        AddonInfomation? AddonInfomation { get; }
        bool IsInitialized { get; }
        bool HasError { get; }
        Exception? Exception { get; }
        IAttemptResult Initialize(AddonInfomation infomation, Action<AddonInfomation, IJavaScriptExecuter> factory);
    }

    public class AddonContext : IAddonContext
    {
        public AddonContext(INicoFileIO fileIO, IAddonLogger addonLogger, IJavaScriptExecuter executer)
        {
            this.fileIO = fileIO;
            this.addonLogger = addonLogger;
            this.Executer = executer;
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
        public IAttemptResult Initialize(AddonInfomation infomation, Action<AddonInfomation, IJavaScriptExecuter> factory)
        {
            if (this.IsInitialized) return new AttemptResult() { Message = "既に初期化されています。" };

            this.AddonInfomation = infomation;
            string code;

            try
            {
                code = this.fileIO.OpenRead(infomation.Scripts.BackgroundScript);
            }
            catch (Exception e)
            {
                this.addonLogger.Error("コードの読み込みに失敗しました。", infomation.Name.Value, e);
                this.Exception = e;
                return new AttemptResult() { Message = "コードの読み込みに失敗しました。", Exception = e };
            }

            factory(this.AddonInfomation, this.Executer);

            _ = Task.Run(() =>
            {
                try
                {
                    this.Executer.Evaluate(code);
                    this.Executer.Evaluate(Const::Adddon.EntryPoint);
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
        }


    }
}
