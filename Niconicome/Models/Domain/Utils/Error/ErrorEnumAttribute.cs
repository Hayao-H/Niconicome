using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAPICodePack.Shell.Interop;

namespace Niconicome.Models.Domain.Utils.Error
{
    [AttributeUsage(AttributeTargets.Field)]
    public class ErrorEnumAttribute : Attribute
    {
        public ErrorEnumAttribute(ErrorLevel errorLevel, string message)
        {
            this.ErrorLevel = errorLevel;
            this.Message = message;
        }

        /// <summary>
        /// エラーレベル
        /// </summary>
        public ErrorLevel ErrorLevel { get; init; }

        /// <summary>
        /// エラーメッセージ
        /// </summary>
        public string Message { get; init; }
    }
}
