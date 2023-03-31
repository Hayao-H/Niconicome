using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niconicome.Models.Domain.Utils.AppEnvironment
{
    public interface IOSInfomationHandler
    {
        /// <summary>
        /// OSバージョンを取得する
        /// </summary>
        /// <returns></returns>
        Version OSversion { get; }

        /// <summary>
        /// OSバージョンを文字列で取得する
        /// </summary>
        /// <returns></returns>
        string OSversionString { get; }

        /// <summary>
        /// 64bitアーキテクチャーであるかどうかを確認
        /// </summary>
        bool Is64BitOperatingSYstem { get; }

        /// <summary>
        /// 作業領域の幅
        /// </summary>
        double WorkAreWidth { get; }

        /// <summary>
        /// 作業領域の高さ
        /// </summary>
        double WorkAreHeight { get; }
    }
}
