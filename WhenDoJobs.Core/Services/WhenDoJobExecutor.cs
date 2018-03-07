using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WhenDoJobs.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Hangfire;
using Microsoft.Extensions.Logging;

namespace WhenDoJobs.Core.Services
{
    public class WhenDoJobExecutor //: IWhenDoJobExecutor
    {
        private IServiceProvider serviceProvider;
        private IBackgroundJobClient jobClient;
        private ILogger<WhenDoJobExecutor> logger;

        public WhenDoJobExecutor(IServiceProvider provider, IBackgroundJobClient jobClient, ILogger<WhenDoJobExecutor> logger)
        {
            this.serviceProvider = provider;
            this.jobClient = jobClient;
            this.logger = logger;
        }

        //public async Task ExecuteAsync(IWhenDoJob job, IWhenDoMessage context)
        //{
        //    foreach (var commandId in job.CommandIds)
        //    {
        //        try
        //        {
        //            var command = repo
        //            switch (command.ExecutionStrategy.Mode)
        //            {
        //                case ExecutionMode.Default:
        //                    var commandExecutor = serviceProvider.GetRequiredService<IWhenDoCommandExecutor>();
        //                    await commandExecutor.ExecuteAsync(context, command.Type, command.MethodName, command.Parameters);
        //                    break;
        //                case ExecutionMode.Reliable:
        //                    jobClient.Enqueue<IWhenDoCommandExecutor>(x => x.ExecuteAsync(context, command.Type, command.MethodName, command.Parameters));
        //                    break;
        //                case ExecutionMode.Delayed:
        //                    jobClient.Schedule<IWhenDoCommandExecutor>(x => x.ExecuteAsync(context, command.Type, command.MethodName, command.Parameters), command.ExecutionStrategy.Time);
        //                    break;
        //                case ExecutionMode.Scheduled:
        //                    var today = DateTime.Today;
        //                    var time = command.ExecutionStrategy.Time;
        //                    var executionTime = (today + time > DateTime.Now) ? today + time : today.AddDays(1) + time;
        //                    jobClient.Schedule<IWhenDoCommandExecutor>(x => x.ExecuteAsync(context, command.Type, command.MethodName, command.Parameters), executionTime);
        //                    logger.LogInformation($"Scheduled command {command.Type} at {time.ToString()}");
        //                    break;
        //            }
        //        }
        //        catch(Exception ex)
        //        {
        //            logger.LogError(ex, $"Error when executing command {command.Type}: {ex.Message}");
        //        }
        //    }
        //}
    }
}
