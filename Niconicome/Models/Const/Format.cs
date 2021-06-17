using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niconicome.Models.Const
{
    class Format
    {
        public const string ISO8601 = "yyyy-MM-ddTHH:mm:ssK";

        public const string FIleFormat = "[<id>]<title>";

        public const string FileExtRegExp = @"^\..+$";

        public const string DefaultIchibaSuffix = "[ichiba]";

        public const string DefaultVideoInfoSuffix = "[info]";

        public const string DefaultFFmpegFormat = "-i \"<source>\" -y -loglevel error -c:a copy -c:v copy \"<output>\"";
    }
}
