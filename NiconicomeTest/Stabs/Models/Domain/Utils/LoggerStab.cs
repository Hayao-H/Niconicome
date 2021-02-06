using System;
using Niconicome.Models.Domain.Utils;

namespace NiconicomeTest.Stabs.Models.Domain.Utils
{
    class LoggerStab : ILogger
    {

        public string? LastMessage { get; private set; }

        public Exception? LastException { get; private set; }

        public void Log(string message)
        {
            this.LastMessage = message;
        }

        public void Log(object message)
        {
            this.LastMessage = message.ToString();
        }

        public void Caution(string message)
        {
            this.LastMessage = message;
        }

        public void Caution(object message)
        {
            this.LastMessage = message.ToString();
        }

        public void Error(string message, Exception exception)
        {
            this.LastMessage = message;
            this.LastException = exception;
        }

        public void Error(string message)
        {
            this.LastMessage = message;
        }
    }


    class LogstreamStab : ILogStream
    {
        public string logContent = string.Empty;

        public string FileName { get => string.Empty; }

        public void WriteAsync(string source)
        {
            this.Write(source);
        }

        public void Write(string source)
        {
            this.logContent = source;
        }
    }
}
