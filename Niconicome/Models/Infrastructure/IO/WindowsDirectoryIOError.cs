using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Utils.Error;

namespace Niconicome.Models.Infrastructure.IO
{
    public enum WindowsDirectoryIOError
    {
        [ErrorEnum(ErrorLevel.Error, "デイレクトリの作成に失敗しました。(path:{0})")]
        FailedToCreateDirectory,
    }
}
