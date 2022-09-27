using System;
using System.IO;
using System.IO.Compression;
using Niconicome.Models.Domain.Utils;
using Niconicome.Models.Helper.Result;
using Const = Niconicome.Models.Const;

namespace Niconicome.Models.Domain.Local.Addons.Core.V2.Install
{
    public interface IAddonExtractor
    {
        /// <summary>
        /// 指定したアドオンファイルを解凍する
        /// </summary>
        /// <param name="archiveFilePath"></param>
        /// <param name="targetDirectory"></param>
        /// <returns></returns>
        IAttemptResult Extract(string archiveFilePath, string targetDirectory);

    }

    public class AddonExtractor : IAddonExtractor
    {
        public AddonExtractor(ILogger logger)
        {
            this._logger = logger;
        }


        #region field

        private readonly ILogger _logger;

        #endregion

        #region Method

        public IAttemptResult Extract(string archiveFilePath, string targetDirectory)
        {

            try
            {
                ZipFile.ExtractToDirectory(archiveFilePath, targetDirectory, true);
            }
            catch (Exception ex)
            {
                this._logger.Error("アドオンファイルの展開に失敗", ex);
                return AttemptResult.Fail($"アドオンファイルの展開に失敗しました。（詳細：{ex.Message}）");
            }

            return AttemptResult.Succeeded();
        }

        #endregion


    }
}
