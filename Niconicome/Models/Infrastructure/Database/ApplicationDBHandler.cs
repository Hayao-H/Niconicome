using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Local.Store.V2;
using Niconicome.Models.Domain.Utils.Error;
using Niconicome.Models.Helper.Result;
using Niconicome.Models.Infrastructure.Database.Error;
using Niconicome.Models.Infrastructure.Database.LiteDB;

namespace Niconicome.Models.Infrastructure.Database
{

    public class ApplicationDBHandler : IApplicationStore
    {
        public ApplicationDBHandler(ILiteDBHandler dataBase, IErrorHandler errorHandler)
        {
            this._dataBase = dataBase;
            this._errorHandler = errorHandler;
        }

        #region field

        private readonly ILiteDBHandler _dataBase;

        private readonly IErrorHandler _errorHandler;

        #endregion

        #region Method

        public IAttemptResult<Version> GetDBVersion()
        {
            IAttemptResult init = this.Initialize();
            if (!init.IsSucceeded) return AttemptResult<Version>.Fail(init.Message);

            IAttemptResult<IReadOnlyList<Types.App>> apps = this._dataBase.GetAllRecords<Types.App>(TableNames.AppInfomation);
            if (!apps.IsSucceeded || apps.Data is null) return AttemptResult<Version>.Fail(apps.Message);

            Types.App app = apps.Data.First();

            if (!Version.TryParse(app.DBVersion, out Version? version))
            {
                this._errorHandler.HandleError(ApplicationDBHandlerError.FailedToParseDBVersion, app.DBVersion);
                return AttemptResult<Version>.Fail(this._errorHandler.GetMessageForResult(ApplicationDBHandlerError.FailedToParseDBVersion, app.DBVersion));
            } else
            {
                return AttemptResult<Version>.Succeeded(version);
            }
        }

        public IAttemptResult SetDBVersion(Version version)
        {
            IAttemptResult init = this.Initialize();
            if (!init.IsSucceeded) return AttemptResult<string>.Fail(init.Message);

            IAttemptResult<IReadOnlyList<Types.App>> apps = this._dataBase.GetAllRecords<Types.App>(TableNames.AppInfomation);
            if (!apps.IsSucceeded || apps.Data is null) return AttemptResult.Fail(apps.Message);

            Types.App app = apps.Data.First();
            app.DBVersion = version.ToString();

            return this._dataBase.Update(app);
        }

        #endregion

        #region private

        /// <summary>
        /// 初期化
        /// </summary>
        private IAttemptResult Initialize()
        {
            IAttemptResult<IReadOnlyList<Types.App>> apps = this._dataBase.GetAllRecords<Types.App>(TableNames.AppInfomation);
            if (!apps.IsSucceeded || apps.Data is null) return AttemptResult.Fail(apps.Message);

            if (apps.Data.Count > 0) return AttemptResult.Succeeded();

            var app = new Types.App();
            return this._dataBase.Insert(app);
        }

        #endregion
    }
}
