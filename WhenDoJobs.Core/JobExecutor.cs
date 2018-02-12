using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhenDoJobs.Core.Interfaces;

namespace WhenDoJobs.Core
{
    public class JobExecutor : IJobExecutor
    {
        IServiceProvider provider;

        public JobExecutor(IServiceProvider provider)
        {
            this.provider = provider;
        }

        public async Task ExecuteAsync(IJob job)
        {
            var result = job.CommandDefinitions.Select(c => c.Build().ExecuteAsync());
            await Task.WhenAll(result);
        }

        public async Task ExecuteCommandAsync(ICommand command)
        {
            await command.ExecuteAsync();
        }
    }
}
