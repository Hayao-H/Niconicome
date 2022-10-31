﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Helper.Result;

namespace Niconicome.Models.Domain.Local.Store
{
    public interface IApplicationDBHandler
    {
        /// <summary>
        /// データベースのバージョンを取得
        /// </summary>
        /// <returns></returns>
        IAttemptResult<Version> GetDBVersion();
    }

    public class ApplicationDBHandler : IApplicationDBHandler
    {
        public ApplicationDBHandler(IDataBase dataBase)
        {
            this._dataBase = dataBase;
        }

        #region field

        private readonly IDataBase _dataBase;

        #endregion

        #region Method

        public IAttemptResult<Version> GetDBVersion()
        {
            IAttemptResult init = this.Initialize();
            if (!init.IsSucceeded) return AttemptResult<Version>.Fail(init.Message);

            IAttemptResult<List<Types.App>> apps = this._dataBase.GetAllRecords<Types.App>(Types.App.TableName);
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
            IAttemptResult<List<Types.App>> apps = this._dataBase.GetAllRecords<Types.App>(Types.App.TableName);
            if (!apps.IsSucceeded || apps.Data is null) return AttemptResult.Fail($"アプリケーション情報データベースの初期化に失敗しました。(詳細：{apps.Message})");

            if (apps.Data.Count > 0) return AttemptResult.Succeeded();

            var app = new Types.App();
            return this._dataBase.Store(app, Types.App.TableName);
        }

        #endregion
    }
}
