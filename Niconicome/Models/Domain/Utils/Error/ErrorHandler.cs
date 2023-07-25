using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Extensions.System;
using Niconicome.Models.Domain.Utils.NicoLogger;
using Const = Niconicome.Models.Const;

namespace Niconicome.Models.Domain.Utils.Error
{
    public interface IErrorHandler
    {
        /// <summary>
        /// 種々のエラー処理を行う
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="items"></param>
        void HandleError<T>(T value, params object[]? items) where T : struct, Enum;

        /// <summary>
        /// 例外情報を伴うエラー処理を行う
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="ex"></param>
        /// <param name="items"></param>
        void HandleError<T>(T value, Exception ex, params object[]? items) where T : struct, Enum;

        /// <summary>
        /// 例外情報を文字列で取得する
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="ex"></param>
        /// <param name="items"></param>
        /// <returns></returns>
        string GetMessageForResult<T>(T value, Exception ex, params object[]? items) where T : struct, Enum;

        /// <summary>
        /// 例外情報を文字列で取得する
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="items"></param>
        /// <returns></returns>
        string GetMessageForResult<T>(T value, params object[]? items) where T : struct, Enum;
    }

    public class ErrorHandler : IErrorHandler
    {
        public ErrorHandler(INiconicomeLogger logger)
        {
            this._logger = logger;
        }

        #region field

        private readonly INiconicomeLogger _logger;

        #endregion

        #region Method

        public void HandleError<T>(T value, params object[]? items) where T : struct, Enum
        {
            if (this.TryGetAttr(value, out ErrorEnumAttribute? attr))
            {
                string errorCode = this.ToErrorCode(value, attr);
                var message = $"[{errorCode}]{this.GetErrorMessage(attr, items ?? new object[0])}";

                this.WriteLog(attr.ErrorLevel, message);
            }
        }

        public void HandleError<T>(T value, Exception ex, params object[]? items) where T : struct, Enum
        {
            if (this.TryGetAttr(value, out ErrorEnumAttribute? attr))
            {
                string errorCode = this.ToErrorCode(value, attr);
                string exMessage = this.GetExceptionMessage(ex);
                var message = $"[{errorCode}] {this.GetErrorMessage(attr, items ?? new object[0])}{Environment.NewLine}{exMessage}";

                this.WriteLog(attr.ErrorLevel, message);
            }
        }

        public string GetMessageForResult<T>(T value, Exception ex, params object[]? items) where T : struct, Enum
        {
            if (this.TryGetAttr(value, out ErrorEnumAttribute? attr))
            {
                return $"{this.GetErrorMessage(attr, items ?? new object[0])}(詳細：{ex.Message})";
            }
            else
            {
                return string.Empty;
            }
        }

        public string GetMessageForResult<T>(T value, params object[]? items) where T : struct, Enum
        {
            if (this.TryGetAttr(value, out ErrorEnumAttribute? attr))
            {
                return $"{this.GetErrorMessage(attr, items ?? new object[0])}";
            }
            else
            {
                return string.Empty;
            }
        }


        #endregion

        #region private

        /// <summary>
        /// エラー属性を取得
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="attr"></param>
        /// <returns></returns>
        private bool TryGetAttr<T>(T value, [NotNullWhen(true)] out ErrorEnumAttribute? attr) where T : struct, Enum => (attr = value.GetType().GetField(value.ToString())?.GetCustomAttribute<ErrorEnumAttribute>()) is not null;

        /// <summary>
        /// エラーコードに変換
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public string ToErrorCode<T>(T value, ErrorEnumAttribute attr) where T : struct, Enum
        {
            var errorLevel = (int)attr.ErrorLevel;
            var errorType = ErrorTypes.ErrorEnums.First(kvpair => kvpair.Value == value.GetType()).Key;
            object? converted;

            try
            {
                converted = Convert.ChangeType(value, Enum.GetUnderlyingType(value.GetType()));
            }
            catch
            {
                return Const::LocalConstant.DefaultErrorCode;
            }

            if (converted is int index)
            {
                return $"{errorLevel}{errorType}-{index}";
            }
            else
            {
                return Const::LocalConstant.DefaultErrorCode;
            }
        }

        /// <summary>
        /// ログを出力
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        private void WriteLog(ErrorLevel errorLevel, string message)
        {

            if (errorLevel == ErrorLevel.Error)
            {
                this._logger.Error(message);
            }
            else if (errorLevel == ErrorLevel.Warning)
            {
                this._logger.Warning(message);
            }
            else
            {
                this._logger.Log(message);
            }
        }

        /// <summary>
        /// メッセージを生成
        /// </summary>
        /// <param name="attr"></param>
        /// <param name="items"></param>
        /// <returns></returns>
        private string GetErrorMessage(ErrorEnumAttribute attr, params object[] items)
        {
            return string.Format(attr.Message, items);
        }

        /// <summary>
        /// 例外情報を取得する
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        private string GetExceptionMessage(Exception ex)
        {
            var message = new StringBuilder();
            var i = 1;
            var space = "  ";

            message.AppendLine(space.Repeat(i) + "例外情報");
            message.AppendLine(space.Repeat(i) + "-".Repeat(50));
            message.AppendLine(space.Repeat(i) + $"Exception Message : {ex.Message}");
            message.AppendLine(space.Repeat(i) + $"Exception Type : {ex.GetType().Name}");
            message.AppendLine(space.Repeat(i) + $"Stack Trace :\n{ex.StackTrace}");

            while (ex.InnerException != null)
            {
                i++;
                ex = ex.InnerException;
                message.AppendLine(space.Repeat(i) + "-".Repeat(20));
                message.AppendLine(space.Repeat(i) + "InnerException");
                message.AppendLine(space.Repeat(i) + $"Exception Message : {ex.Message}");
                message.AppendLine(space.Repeat(i) + $"Exception Type : {ex.GetType().Name}");
                message.AppendLine(space.Repeat(i) + $"Stack Trace :\n{ex.StackTrace}");
                message.AppendLine(space.Repeat(i) + "-".Repeat(20));
            }
            message.AppendLine("-".Repeat(50));

            return message.ToString();
        }

        #endregion
    }
}
