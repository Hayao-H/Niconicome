using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Extensions.System;
using Niconicome.Models.Domain.Local.Store.Types;
using Niconicome.Models.Domain.Utils;
using Niconicome.Models.Helper.Result;
using Niconicome.Models.Helper.Result.Generic;
using Windows.Devices.Pwm;

namespace Niconicome.Models.Domain.Local.Addons.Core.Installer
{
    public interface IAddonStoreHandler
    {
        AddonInfomation? GetAddon(Expression<Func<Addon, bool>> predicate);
        bool IsInstallled(Expression<Func<Addon, bool>> predicate);
        IAttemptResult<int> StoreAddon(AddonInfomation addon);
        IAttemptResult<int> Update(AddonInfomation addon);

        /// <summary>
        /// 指定した条件でアドオンを削除する
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        IAttemptResult Delete(Expression<Func<Addon, bool>> predicate);
    }

    public class AddonStoreHandler : IAddonStoreHandler
    {
        public AddonStoreHandler(IAddonInfomationsContainer infomationsContainer, IDataBase dataBase, ILogger logger)
        {
            this.container = infomationsContainer;
            this.dataBase = dataBase;
            this.logger = logger;
        }

        #region field

        private readonly IAddonInfomationsContainer container;

        private readonly IDataBase dataBase;

        private readonly ILogger logger;

        #endregion

        /// <summary>
        /// アドオンを取得する
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public AddonInfomation? GetAddon(Expression<Func<Addon, bool>> predicate)
        {
            IAttemptResult<Addon> result = this.dataBase.GetRecord(Addon.TableName, predicate);

            if (!result.IsSucceeded || result.Data is null)
            {
                if (result.Exception is not null)
                {
                    this.logger.Error("アドオンの取得に失敗しました。", result.Exception);
                    return null;
                }
                else
                {
                    this.logger.Error($"アドオンの取得に失敗しました。(詳細:{result.Message})");
                    return null;
                }
            }

            AddonInfomation infomation = this.container.GetAddon(result.Data.Id);
            this.SetLocalData(infomation, result.Data);

            return infomation;
        }

        /// <summary>
        /// アドオンを保存する
        /// </summary>
        /// <param name="addon"></param>
        /// <returns></returns>
        public IAttemptResult<int> StoreAddon(AddonInfomation addon)
        {
            if (string.IsNullOrEmpty(addon.Identifier.Value))
            {
                if (this.IsInstallled(db => db.Name == addon.Name.Value))
                {
                    return new AttemptResult<int>() { Message = $"{addon.Name.Value}は既にインストールされています。" };
                }
            }
            else
            {
                //任意識別子が指定されている場合
                if (this.IsInstallled(d => d.Identifier == addon.Identifier.Value))
                {
                    return new AttemptResult<int>() { Message = $"{addon.Identifier.Value}は既にインストールされています。" };
                }
            }

            var data = new Addon();
            this.SetDBData(addon, data);

            IAttemptResult<int> result = this.dataBase.Store(data, Addon.TableName);

            if (!result.IsSucceeded)
            {
                if (result.Exception is not null)
                {
                    this.logger.Error("アドオンの追加に失敗しました。", result.Exception);
                }
                else
                {
                    this.logger.Error($"アドオンの追加に失敗しました。(詳細:{result.Message})");
                }

                return result;
            }

            return new AttemptResult<int>() { IsSucceeded = true, Data = result.Data };
        }

        /// <summary>
        /// アドオンを更新する
        /// </summary>
        /// <param name="addon"></param>
        /// <returns></returns>
        public IAttemptResult<int> Update(AddonInfomation addon)
        {
            if (!this.IsInstallled(a => addon.Identifier.Value.IsNullOrEmpty() ? a.Id == addon.ID.Value : a.Identifier == addon.Identifier.Value))
            {
                return new AttemptResult<int>() { Message = "アドオンがインストールされていません。" };
            }

            var db = new Addon();
            this.SetDBData(addon, db);

            IAttemptResult result = this.dataBase.Update(db, Addon.TableName);
            if (!result.IsSucceeded)
            {
                if (result.Exception is not null)
                {
                    this.logger.Error("アドオンの更新に失敗しました。", result.Exception);
                }
                else
                {
                    this.logger.Error($"アドオンの更新に失敗しました。(詳細:{result.Message})");
                }

                return new AttemptResult<int>() { Message = result.Message, Exception = result.Exception };
            }

            return new AttemptResult<int>() { IsSucceeded = true, Data = addon.ID.Value };
        }

        public IAttemptResult Delete(Expression<Func<Addon, bool>> predicate)
        {
            IAttemptResult<bool> result = this.dataBase.DeleteAll(Addon.TableName, predicate);

            if (!result.IsSucceeded)
            {
                if (result.Exception is not null)
                {
                    this.logger.Error($"アドオンの削除に失敗しました(詳細:{result.Message})", result.Exception);
                }
                else
                {
                    this.logger.Error($"アドオンの削除に失敗しました(詳細:{result.Message})");
                }
            }

            return new AttemptResult() { IsSucceeded = result.Data, Message = result.Message };

        }


        /// <summary>
        /// IDからインストールされているかどうかをチェックする
        /// </summary>
        /// <param name="packageID"></param>
        /// <returns></returns>
        public bool IsInstallled(Expression<Func<Addon, bool>> predicate)
        {
            bool result = this.dataBase.Exists<Addon>(Addon.TableName, predicate);
            return result;
        }

        #region private

        /// <summary>
        /// ローカルデータにセットする
        /// </summary>
        /// <param name="infomation"></param>
        /// <param name="addon"></param>
        private void SetLocalData(AddonInfomation infomation, Addon addon)
        {
            infomation.ID.Value = addon.Id;
            infomation.Name.Value = addon.Name;
            infomation.Author.Value = addon.Author;
            infomation.Description.Value = addon.Description;
            infomation.Version.Value = Version.Parse(addon.Version);
            infomation.Identifier.Value = addon.Identifier;
            infomation.Permissions.AddRange(addon.Permissions);
            infomation.TargetAPIVersion.Value = Version.Parse(addon.TargetAPIVersion);
        }

        /// <summary>
        /// DBのデータにセットする
        /// </summary>
        /// <param name="infomation"></param>
        /// <param name="addon"></param>
        private void SetDBData(AddonInfomation infomation, Addon addon)
        {
            addon.Id = infomation.ID.Value;
            addon.Name = infomation.Name.Value;
            addon.Author = infomation.Author.Value;
            addon.Description = infomation.Description.Value;
            addon.Version = infomation.Version.Value.ToString();
            addon.Identifier = infomation.Identifier.Value;
            addon.Permissions = infomation.Permissions;
            addon.PackageID = infomation.PackageID.Value;
            addon.TargetAPIVersion = infomation.TargetAPIVersion.Value.ToString();
        }

        #endregion
    }
}
