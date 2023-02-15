using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Extensions.System.Diagnostics;
using Niconicome.Models.Domain.Utils.Error;
using Niconicome.Models.Helper.Result;
using Niconicome.Models.Local.External.Error;

namespace Niconicome.Models.Local.External
{
    public interface IExternalProcessUtils
    {
        IAttemptResult StartProcess(string arg);
    }

    public class ExternalProcessUtils : IExternalProcessUtils
    {

        public ExternalProcessUtils(IErrorHandler errorHandler)
        {
            this._errorHandler = errorHandler;
        }

        private readonly IErrorHandler _errorHandler;

        public IAttemptResult StartProcess(string arg)
        {
            try
            {
                ProcessEx.StartWithShell(arg);
            }
            catch (Exception ex)
            {
                this._errorHandler.HandleError(ExternalProcessUtilsError.FailedToStartProcess, ex, arg);
                return AttemptResult.Fail(this._errorHandler.GetMessageForResult(ExternalProcessUtilsError.FailedToStartProcess, ex, arg));
            }

            return AttemptResult.Succeeded();
        }
    }
}
