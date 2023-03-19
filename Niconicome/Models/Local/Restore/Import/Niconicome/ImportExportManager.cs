using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Local.DataBackup.Import.Niconicome;
using Niconicome.Models.Helper.Result;

namespace Niconicome.Models.Local.Restore.Import.Niconicome
{
    public interface IImportExportManager
    {
        /// <summary>
        /// データをエクスポート
        /// </summary>
        /// <returns></returns>
        IAttemptResult ExportData();

        /// <summary>
        /// データをインポート
        /// </summary>
        /// <param name="pathOfJson"></param>
        /// <returns></returns>
        IAttemptResult<ImportResult> ImportData(string path);
    }

    public class ImportExportManager : IImportExportManager
    {
        public ImportExportManager(IExportHandler exportHandler, IImportHandler importHandler)
        {
            this._exportHandler = exportHandler;
            this._importHandler = importHandler;
        }

        #region field

        private readonly IExportHandler _exportHandler;

        private readonly IImportHandler _importHandler;

        #endregion

        #region Method

        public IAttemptResult ExportData()
        {
            return this._exportHandler.ExportData();
        }

        public IAttemptResult<ImportResult> ImportData(string path)
        {
            return this._importHandler.ImportData(path);
        }

        #endregion
    }
}
