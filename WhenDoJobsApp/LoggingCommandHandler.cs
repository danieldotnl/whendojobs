using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WhenDoJobs.Core.Interfaces;

namespace WhenDoJobsApp
{
    public class LoggingCommandHandler : ILoggingCommandHandler
    {
        private ILogger<LoggingCommandHandler> logger;

        public LoggingCommandHandler(ILogger<LoggingCommandHandler> logger)
        {
            this.logger = logger;
        }

        public Task LogErrorAsync(string text)
        {
            logger.LogError(text);
            return Task.CompletedTask;
        }

        public Task LogWarningAsync(string text)
        {
            logger.LogWarning(text);
            return Task.CompletedTask;
        }

        public Task LogDebugAsync(string text)
        {
            logger.LogDebug(text);
            return Task.CompletedTask;
        }

        public Task LogInformationAsync(string text)
        {
            logger.LogInformation(text);
            return Task.CompletedTask;
        }

        public Task LogTraceAsync(string text)
        {
            logger.LogTrace(text);
            return Task.CompletedTask;
        }

        public Task LogCriticalAsync(string text)
        {
            logger.LogCritical(text);
            return Task.CompletedTask;
        }
    }
}
