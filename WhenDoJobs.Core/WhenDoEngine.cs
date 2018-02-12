using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WhenDoJobs.Core.Interfaces;
using System.Threading;
using System.Reflection;
using System.Linq;
using System.Linq.Expressions;
using System.Linq.Dynamic.Core;
using WhenDoJobs.Core.Models;

namespace WhenDoJobs.Core
{
    public class WhenDoEngine : IWhenDoEngine
    {
        private IQueueProvider persistence;
        private ILogger<WhenDoEngine> logger;
        private IServiceProvider serviceProvider;
        private List<IJob> jobs = new List<IJob>();

        public WhenDoEngine(IQueueProvider persistence, IServiceProvider serviceProvider, ILogger<WhenDoEngine> logger)
        {
            this.persistence = persistence;
            this.logger = logger;
            this.serviceProvider = serviceProvider;
        }

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Start listing to queue");
            while (!cancellationToken.IsCancellationRequested)
            {
                if (persistence.GetMessage(out IMessageContext message))
                {
                    logger.LogTrace("Message received", message);
                    await HandleMessage(message);                    
                    continue;
                }
                await Task.Delay(1000);
            }
        }

        public async Task HandleMessage(IMessageContext message)
        {
            var executableJobs = jobs.Where(x => x.Evaluate(message) && x.IsRunnable());

            try
            {
                var tasks = new List<Task>();
                foreach (var job in executableJobs)
                {
                    var jobExecutor = serviceProvider.GetRequiredService<IJobExecutor>();
                    var result = jobExecutor.ExecuteAsync(job);
                    tasks.Add(result);
                }
                await Task.WhenAll(tasks);
            }
            catch (Exception ex)
            {
                logger.LogError("Error processing queue message", ex, message);
            }
        }

        public void RegisterJob(JobDefinition template)
        {
            var mi = this.GetType().GetMethod("RegisterJobDefinition", new Type[] { typeof(JobDefinition) });
            var type = Type.GetType($"WhenDoJobsApp.Messages.{template.Context}, WhenDoJobsApp"); //TODO: make generic
            var fooRef = mi.MakeGenericMethod(type);
            fooRef.Invoke(this, new object[] { template });            
        }

        public void RegisterJobDefinition<T>(JobDefinition jobDefinition) where T: IMessageContext
        {
            var job = jobDefinition.ToJob<T>(serviceProvider.GetRequiredService<IJob>());
            jobs.Add(job);
        }       
    }
}
