using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Utils.Error;

namespace Niconicome.Models.Local.Restore
{
    public enum RestoreManagerError
    {
        [ErrorEnum(ErrorLevel.Error,"指定されたディレクトリはすでに登録済みです。(path:{0})")]
        VideoDirectoryAllreadyRegistered,
        [ErrorEnum(ErrorLevel.Error, "指定されたディレクトリは登録されていません。(path:{0})")]
        VideoDirectoryNotRegistered,
    }
}
