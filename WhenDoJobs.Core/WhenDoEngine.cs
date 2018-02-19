using Hangfire;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using WhenDoJobs.Core.Interfaces;
using WhenDoJobs.Core.Models;
using WhenDoJobs.Core.Services;

namespace WhenDoJobs.Core
{
    public class WhenDoEngine : IWhenDoEngine
    {
        private IQueueProvider persistence;
        private ILogger<WhenDoEngine> logger;
        private IServiceProvider serviceProvider;
        private IDateTimeProvider dateTimeProvider;
        private IWhenDoRegistry registry;
        private WhenDoConfiguration config;
        private BackgroundJobServer hangfireServer;
        private JobStorage hangfireStorage;

        public WhenDoEngine(IQueueProvider persistence, IServiceProvider serviceProvider,
            ILogger<WhenDoEngine> logger, IDateTimeProvider dateTimeProvider, IWhenDoRegistry registry, WhenDoConfiguration config, JobStorage hangfireStorage)
        {
            this.persistence = persistence;
            this.logger = logger;
            this.serviceProvider = serviceProvider;
            this.dateTimeProvider = dateTimeProvider;
            this.registry = registry;
            this.config = config;
            this.hangfireStorage = hangfireStorage;
        }

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            if (!config.ExternalHangfireServer)
            {
                try
                {
                    GlobalConfiguration.Configuration.UseActivator(new HangfireJobActivator(serviceProvider));
                    this.hangfireServer = new BackgroundJobServer(hangfireStorage);
                }
                catch(Exception ex)
                {
                    logger.LogError("Error starting hangfire server", ex);
                }
            }

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
            if (hangfireServer != null)
            {
                hangfireServer.Dispose();
            }
        }

        public async Task HandleMessage(IMessageContext message)
        {
            try
            {
                var executableJobs = registry.Jobs.Where(x => x.Evaluate(message) && x.IsRunnable(dateTimeProvider)).ToList();
                if (executableJobs.Count > 0)
                {
                    var jobExecutor = serviceProvider.GetRequiredService<IWhenDoJobExecutor>();
                    //TODO: foreachasync extension method??
                    var tasks = new List<Task>();
                    executableJobs.ForEach((x) => tasks.Add(jobExecutor.ExecuteAsync(x)));
                    await Task.WhenAll(tasks);
                }
                else
                    logger.LogInformation("No jobs to be executed for message {message}", message);
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

        public void RegisterCommandHandler<T>(string type) where T : class, IWhenDoCommandHandler
        {
            registry.RegisterCommandHandler<T>(type);
        }

        public void ClearJobRegister()
        {
            registry.ClearJobRegister();
        }
    }
}
