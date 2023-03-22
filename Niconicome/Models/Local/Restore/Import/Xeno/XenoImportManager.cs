using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Local.DataBackup.Import.Xeno;
using Niconicome.Models.Helper.Result;

namespace Niconicome.Models.Local.Restore.Import.Xeno
{
    public interface IXenoImportManager
    {
        /// <summary>
        /// Xenoからデータをインポート
        /// </summary>
        /// <param name="path"></param>
        /// <param name="onMessage"></param>
        /// <returns></returns>
        Task<IAttemptResult<IXenoImportResult>> ImportDataAsync(string path, Action<string> onMessage);
    }

    internal class XenoImportManager : IXenoImportManager
    {
        public XenoImportManager(IXenoImportHandler handler)
        {
            this._handler = handler;
        }

        private readonly IXenoImportHandler _handler;

        public async Task<IAttemptResult<IXenoImportResult>> ImportDataAsync(string path, Action<string> onMessage)
        {
            return await Task.Run(() => this._handler.ImportData(path, onMessage));
        }

    }
}
