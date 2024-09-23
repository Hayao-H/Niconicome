using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niconicome.Models.Local.Application
{
    public interface IOSThemeHandler
    {
        /// <summary>
        /// 互換性チェック
        /// </summary>
        bool FunctionHasCompatibility { get; }

        /// <summary>
        /// ダークモードかどうか
        /// </summary>
        bool IsDarkMode { get; }

        /// <summary>
        /// テーマが変更されたとき
        /// </summary>
        event EventHandler ThemeChanged;
    }
}
