using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Helper.Result;

namespace Niconicome.Models.Domain.Local.IO.Media.Audio
{
    public interface IAudioPlayer
    {
        /// <summary>
        /// 音声ファイルを再生する
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        IAttemptResult Play(string filePath);
    }
}
