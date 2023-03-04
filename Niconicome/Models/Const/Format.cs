using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niconicome.Models.Const
{
    class Format
    {
        public const string ISO8601 = "yyyy-MM-ddTHH:mm:ssK";

        public const string DefaultFileNameFormat = "[<id>]<title>";

        public const string FileExtRegExp = @"^\..+$";

        public const string DefaultIchibaSuffix = "[ichiba]";

        public const string DefaultVideoInfoSuffix = "[info]";

        public const string DefaultFFmpegFormat = "-i \"<source>\" -y -loglevel error -c:a copy -c:v copy \"<output>\"";

        public const string DefaultOwnerCommentSuffix = "[owner]";

        public const string DefaultEconomyVideoSuffix = "[economy]";

        public const string DefaultThumbnailSuffix = "";

        public const string FolderAutoMapSymbol = "<autoMap>";

        public const string FFmpegPath = @"ffmpeg\bin\ffmpeg.exe";
    }
}
