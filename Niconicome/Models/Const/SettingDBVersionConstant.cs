using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niconicome.Models.Const
{
    public class SettingDBVersionConstant
    {
        /// <summary>
        /// 現在のDBバージョン
        /// </summary>
        public static Version CurrentSettingDBVersion = new("1.0.0");

        /// <summary>
        /// 市場関係の設定を修正
        /// </summary>
        public static Version IchibaSettingFixed = new("1.0.0");
    }
}
