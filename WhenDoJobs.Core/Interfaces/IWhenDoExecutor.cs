using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace WhenDoJobs.Core.Interfaces
{
    public interface IWhenDoExecutor
    {
        Task ExecuteJobAsync(IJob job);
        Task ExecuteCommandAsync(ICommand command);
    }
}
