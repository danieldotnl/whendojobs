using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WhenDoJobs.Core.Interfaces
{
    public interface IWhenDoEngine
    {
        Task RunAsync(CancellationToken cancellationToken);
        Task HandleMessage(IMessageContext message);
    }
}
