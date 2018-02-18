using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace WhenDoJobs.Core.Tests.Helpers
{
    public class Logger<T> : ILogger<T>
    {
        public int Count { get; set; } = 0;

        public IDisposable BeginScope<TState>(TState state)
        {
            throw new NotImplementedException();
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            throw new NotImplementedException();
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            try
            {
                Count++;

            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public void LogError(string text)
        {
            Count++;
        }
    }
}
