using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WhenDoJobs.Core.Interfaces;

namespace WhenDoJobsApp
{
    public interface ILoggingCommandHandler : IWhenDoCommandHandler
    {
        Task LogErrorAsync(string text);
        Task LogDebugAsync(string text);
        Task LogWarningAsync(string text);
        Task LogInformationAsync(string text);
        Task LogTraceAsync(string text);
        Task LogCriticalAsync(string text);
    }
}
