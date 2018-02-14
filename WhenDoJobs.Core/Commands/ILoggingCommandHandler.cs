using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WhenDoJobs.Core.Interfaces;

namespace WhenDoJobs.Core.Commands
{
    public interface ILoggingCommandHandler : ICommandHandler
    {
        Task LogError(string text);
        Task LogDebug(string text);
        Task LogInformation(string text);
        Task LogTrace(string text);
        Task LogCritical(string text);
    }
}
