using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Utils.Error;

namespace Niconicome.Models.Domain.Local.LocalFile.Error
{
    public enum LocalFileRemoverError
    {
        [ErrorEnum(ErrorLevel.Error,"実体ファイルを削除しました。(file：{0})")]
        RemovedFile,
    }
}
