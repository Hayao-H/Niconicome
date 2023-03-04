using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Const;
using Niconicome.Models.Domain.Local.IO.V2;
using Niconicome.Models.Helper.Result;
using Niconicome.Properties;

namespace Niconicome.Models.Local.State.Style
{
    public interface IUserChromeHandler
    {
        /// <summary>
        /// スタイルシートを書き出し
        /// </summary>
        /// <returns></returns>
        IAttemptResult SaveStyle();
    }

    public class UserChromeHandler : IUserChromeHandler
    {
        public UserChromeHandler(INiconicomeDirectoryIO directoryIO, INiconicomeFileIO fileIO)
        {
            this._fileIO = fileIO;
            this._directoryIO = directoryIO;
        }

        #region field

        private readonly INiconicomeFileIO _fileIO;

        private readonly INiconicomeDirectoryIO _directoryIO;

        #endregion

        #region Method

        public IAttemptResult SaveStyle()
        {
            string content = Resources.UserChrome;

            IAttemptResult dirResult = this.CreateDirectory();
            if (!dirResult.IsSucceeded)
            {
                return dirResult;
            }

            string path = Path.Join(AppContext.BaseDirectory, FileFolder.UserChromePath);

            return this._fileIO.Write(path, content, Encoding.UTF8);
        }

        #endregion

        #region private

        private IAttemptResult CreateDirectory()
        {
            if (this._directoryIO.Exists("chrome"))
            {
                return AttemptResult.Succeeded();
            }

            return this._directoryIO.CreateDirectory("chrome");
        }

        #endregion
    }
}
