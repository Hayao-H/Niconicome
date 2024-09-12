using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Utils.Error;

namespace NiconicomeTest.Stabs.Models.Domain.Utils.Error
{
    public class ErrorHandlerStub : IErrorHandler
    {
        public string HandleError<T>(T value, params object[]? items) where T : struct, Enum
        {
            return string.Empty;
        }

        public string HandleError<T>(T value, Exception ex, params object[]? items) where T : struct, Enum
        {
            return string.Empty;
        }

        public string GetMessageForResult<T>(T value, Exception ex, params object[]? items) where T : struct, Enum
        {
            return string.Empty;
        }

        public string GetMessageForResult<T>(T value, params object[]? items) where T : struct, Enum
        {
            return string.Empty;
        }
    }
}
