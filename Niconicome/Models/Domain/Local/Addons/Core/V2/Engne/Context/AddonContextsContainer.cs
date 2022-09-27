using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Niconicome.Models.Domain.Local.Addons.Core.V2.Engine.Infomation;
using Niconicome.Models.Domain.Utils;
using Niconicome.Models.Helper.Result;

namespace Niconicome.Models.Domain.Local.Addons.Core.V2.Engine.Context
{
    public interface IAddonContextsContainer
    {
        /// <summary>
        /// コンテキストを追加する
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        IAttemptResult Add(IAddonContext context);

        /// <summary>
        /// 指定したIDに合致する<see cref="IAddonInfomation">IAddonInfomation</see>を持つコンテキストを取得する
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        IAttemptResult<IAddonContext> Get(string ID);

        /// <summary>
        /// 特定のコンテキストを終了する
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        IAttemptResult ShutDown(string ID);

        /// <summary>
        /// コンテキストを生成
        /// </summary>
        /// <returns></returns>
        IAttemptResult<IAddonContext> Create();
    }

    public class AddonContextsContainer : IAddonContextsContainer
    {
        public AddonContextsContainer(ILogger logger)
        {
            this.logger = logger;
        }

        #region field

        private readonly List<IAddonContext> contexts = new();

        private readonly ILogger logger;

        #endregion

        #region Method

        public IAttemptResult Add(IAddonContext context)
        {
            if (context.AddonInfomation is null)
            {
                return AttemptResult.Fail($"{nameof(IAddonContext)}の{nameof(IAddonContext.AddonInfomation)}がnullです。");
            }
            else if (this.Contains(context))
            {
                return AttemptResult.Fail("与えられたオブジェクトはすでに追加されています。");
            }
            else
            {
                this.contexts.Add(context);
                return AttemptResult.Succeeded();
            }
        }

        public IAttemptResult<IAddonContext> Get(string ID)
        {
            IAddonContext? context = this.contexts.FirstOrDefault(c => c.AddonInfomation!.ID == ID);

            if (context is null)
            {
                return AttemptResult<IAddonContext>.Fail("指定されたIDを持つオブジェクトは存在しません。");
            }
            else
            {
                return AttemptResult<IAddonContext>.Succeeded(context);
            }
        }

        public IAttemptResult ShutDown(string ID)
        {
            IAttemptResult<IAddonContext> getResult = this.Get(ID);
            if (!getResult.IsSucceeded || getResult.Data is null)
            {
                return AttemptResult.Fail(getResult.Message);
            }

            IAddonContext context = getResult.Data;
            context.ShutDown();

            this.contexts.Remove(context);

            return AttemptResult.Succeeded();
        }

        public IAttemptResult<IAddonContext> Create()
        {
            IAddonContext? context;

            try
            {
                context = DIFactory.Provider.GetRequiredService<IAddonContext>();
            }
            catch (Exception ex)
            {
                this.logger.Error("コンテキストの生成に失敗しました。", ex);
                return AttemptResult<IAddonContext>.Fail($"コンテキストの生成に失敗しました。(詳細：{ex.Message})");
            }


            return AttemptResult<IAddonContext>.Succeeded(context);
        }




        #endregion

        #region private

        private bool Contains(IAddonContext context)
        {
            if (this.contexts.Contains(context))
            {
                return true;
            }
            else if (this.contexts.Any(c => c.AddonInfomation!.ID == context.AddonInfomation!.ID))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion
    }
}
