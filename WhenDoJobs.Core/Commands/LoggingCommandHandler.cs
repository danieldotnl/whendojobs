using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WhenDoJobs.Core.Interfaces;

namespace WhenDoJobs.Core.Commands
{
    public class LoggingCommandHandler : ILoggingCommandHandler
    {
        private ILogger<LoggingCommandHandler> logger;

        public LoggingCommandHandler(ILogger<LoggingCommandHandler> logger)
        {
            this.logger = logger;
        }

        public Task LogError(string text)
        {
            logger.LogError(text);
            return Task.CompletedTask;
        }

        public Task LogDebug(string text)
        {
            logger.LogDebug(text);
            return Task.CompletedTask;
        }

        public Task LogInformation(string text)
        {
            logger.LogInformation(text);
            return Task.CompletedTask;
        }

        public Task LogTrace(string text)
        {
            logger.LogTrace(text);
            return Task.CompletedTask;
        }

        public Task LogCritical(string text)
        {
            logger.LogCritical(text);
            return Task.CompletedTask;
        }
    }
}
