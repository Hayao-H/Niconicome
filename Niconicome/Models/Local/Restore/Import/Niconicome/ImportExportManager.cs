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
        Task<IAttemptResult<string>> ExportDataAsync();

        /// <summary>
        /// データをインポート
        /// </summary>
        /// <param name="pathOfJson"></param>
        /// <returns></returns>
        Task<IAttemptResult<ImportResult>> ImportDataAsync(string path);
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

        public async Task<IAttemptResult<string>> ExportDataAsync()
        {
            return await Task.Run(() => this._exportHandler.ExportData());
        }

        public async Task<IAttemptResult<ImportResult>> ImportDataAsync(string path)
        {
            return await Task.Run(() => this._importHandler.ImportData(path));
        }

        #endregion
    }
}
