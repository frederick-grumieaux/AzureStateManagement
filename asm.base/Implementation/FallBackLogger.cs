using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Asm.Implementation
{
    public class FallBackLogger : Asm.IFallBackLogger
    {
        public IDisposable BeginScope<TState>(TState state)
        {
            return Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance.BeginScope<TState>(state);
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance.IsEnabled(logLevel);
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance.Log<TState>(logLevel, eventId, state, exception, formatter);
        }
    }
}
