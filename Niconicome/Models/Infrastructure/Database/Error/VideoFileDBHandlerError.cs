using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Utils.Error;

namespace Niconicome.Models.Infrastructure.Database.Error
{
    public enum VideoFileDBHandlerError
    {
        [ErrorEnum(ErrorLevel.Error, "動画ファイル検索中にエラーが発生しました。(path:{0})")]
        ErrorWhenEnumerateVideoFiles,
    }
}
