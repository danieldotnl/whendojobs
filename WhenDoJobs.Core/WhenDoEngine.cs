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
        private IDateTimeProvider dateTimeProvider;
        private IWhenDoRegistry registry;

        public WhenDoEngine(IQueueProvider persistence, IServiceProvider serviceProvider, 
            ILogger<WhenDoEngine> logger, IDateTimeProvider dateTimeProvider, IWhenDoRegistry registry)
        {
            this.persistence = persistence;
            this.logger = logger;
            this.serviceProvider = serviceProvider;
            this.dateTimeProvider = dateTimeProvider;
            this.registry = registry;
        }

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            registry.BuildServiceProvider();
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
            try
            {
                var executableJobs = registry.Jobs.Where(x => x.Evaluate(message) && x.IsRunnable(dateTimeProvider));

                var tasks = new List<Task>();
                foreach (var job in executableJobs)
                {
                    var executor = serviceProvider.GetRequiredService<IWhenDoExecutor>();
                    var result = executor.ExecuteJobAsync(job);
                    tasks.Add(result);
                }
                await Task.WhenAll(tasks);
            }
            catch (Exception ex)
            {
                logger.LogError("Error processing queue message: {error}", ex, message);
            }
        }
  
        public void RegisterJob(JobDefinition jobDefinition)
        {
            registry.RegisterJob(jobDefinition.ToJob());
        }

        public void RegisterCommandHandler<T>(string type) where T : class, ICommandHandler
        {
            registry.RegisterCommandHandler<T>(type);
        }
    }
}
