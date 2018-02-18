using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WhenDoJobs.Core.Models;

namespace WhenDoJobs.Core.Interfaces
{
    public interface IWhenDoEngine
    {
        Task RunAsync(CancellationToken cancellationToken);
        Task HandleMessage(IMessageContext message);

        void RegisterJob(JobDefinition template);
        void RegisterCommandHandler<T>(string type)
            where T : class, IWhenDoCommandHandler;
    }
}
