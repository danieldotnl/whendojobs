using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WhenDoJobs.Core.Models;

namespace WhenDoJobs.Core.Interfaces
{
    public interface IWhenDoJobManager
    {
        Task RegisterJobAsync(JobDefinition jobDefinition);
        Task RegisterJobAsync(IWhenDoJob job);
        Task ClearJobsAsync();
    }
}
