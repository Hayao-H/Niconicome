using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Local.Store.V2;
using Niconicome.Models.Helper.Result;
using Niconicome.Models.Infrastructure.Database.LiteDB;

namespace Niconicome.Models.Infrastructure.Database
{

    public class ApplicationDBHandler : IApplicationStore
    {
        public ApplicationDBHandler(ILiteDBHandler dataBase)
        {
            this._dataBase = dataBase;
        }

        #region field

        private readonly ILiteDBHandler _dataBase;

        #endregion

        #region Method

        public IAttemptResult<Version> GetDBVersion()
        {
            IAttemptResult init = this.Initialize();
            if (!init.IsSucceeded) return AttemptResult<Version>.Fail(init.Message);

            IAttemptResult<IReadOnlyList<Types.App>> apps = this._dataBase.GetAllRecords<Types.App>(TableNames.AppInfomation);
            if (!apps.IsSucceeded || apps.Data is null) return AttemptResult<Version>.Fail(apps.Message);

            Types.App app = apps.Data.First();
            Version.TryParse(app.DBVersion, out Version? version);
            return version is null ? AttemptResult<Version>.Fail("DBバージョン文字列の解析に失敗しました。") : AttemptResult<Version>.Succeeded(version);

        }

        #endregion

        #region private

        /// <summary>
        /// 初期化
        /// </summary>
        private IAttemptResult Initialize()
        {
            IAttemptResult<IReadOnlyList<Types.App>> apps = this._dataBase.GetAllRecords<Types.App>(TableNames.AppInfomation);
            if (!apps.IsSucceeded || apps.Data is null) return AttemptResult.Fail($"アプリケーション情報データベースの初期化に失敗しました。(詳細：{apps.Message})");

            if (apps.Data.Count > 0) return AttemptResult.Succeeded();

            var app = new Types.App();
            return this._dataBase.Insert(app);
        }

        #endregion
    }
}
