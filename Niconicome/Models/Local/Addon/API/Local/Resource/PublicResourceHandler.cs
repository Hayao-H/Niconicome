using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Local.Addons.API.Resource;
using Niconicome.Models.Domain.Local.Addons.Core;
using Niconicome.Models.Domain.Local.Addons.Core.V2.Engine.Infomation;
using Niconicome.Models.Helper.Result;

namespace Niconicome.Models.Local.Addon.API.Local.Resource
{
    public interface IPublicResourceHandler
    {
        /// <summary>
        /// リソースを取得する
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        string? getResource(string relativePath);

        /// <summary>
        /// 初期化する
        /// </summary>
        /// <param name="info"></param>
        void Initialize(IAddonInfomation info);
    }

    public class PublicResourceHandler : IPublicResourceHandler
    {
        public PublicResourceHandler(IResourceHander hander)
        {
            this._handler = hander;
        }

        #region field

        private readonly IResourceHander _handler;

        #endregion

        public string? getResource(string relativePath)
        {
            IAttemptResult<string> result = this._handler.GetResource(relativePath);
            if (!result.IsSucceeded)
            {
                return null;
            }
            else
            {
                return result.Data;
            }
        }


        public void Initialize(IAddonInfomation info)
        {
            this._handler.Initialize(info.DirectoryName, info.Name);
        }

    }
}
