using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WhenDoJobs.Core.Interfaces;

namespace WhenDoJobs.Core.Commands
{
    public class LoggingCommand : ICommand
    {
        private ILogger<LoggingCommand> logger;

        public LogLevel Level { get; set; }
        public string Text { get; set; }

        public LoggingCommand(ILogger<LoggingCommand> logger)
        {
            this.logger = logger;
        }

        public Task ExecuteAsync()
        {
            switch (this.Level)
            {
                case LogLevel.Trace:
                    logger.LogTrace(this.Text);
                    break;
                case LogLevel.Debug:
                    logger.LogDebug(this.Text);
                    break;
                case LogLevel.Information:
                    logger.LogInformation(this.Text);
                    break;
                case LogLevel.Warning:
                    logger.LogWarning(this.Text);
                    break;
                case LogLevel.Error:
                    logger.LogError(this.Text);
                    break;
                case LogLevel.Critical:
                    logger.LogCritical(this.Text);
                    break;
                default:
                    break;
            }

            return Task.CompletedTask;
        }
    }
}
