using Hangfire;
using Hangfire.Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using WhenDoJobs.Core.Interfaces;
using WhenDoJobs.Core.Models;
using WhenDoJobs.Core.Services;

namespace WhenDoJobs.Core
{
    public class WhenDoEngine : IWhenDoEngine
    {
        private IWhenDoQueueProvider queue;
        private ILogger<WhenDoEngine> logger;
        private IServiceProvider serviceProvider;
        private IWhenDoRegistry registry;
        private WhenDoConfiguration config;
        private BackgroundJobServer hangfireServer;
        private JobStorage hangfireStorage;
        private IWhenDoJobExecutionManager jobExecutionManager;
        private IWhenDoJobManager jobManager;

        public WhenDoEngine(IWhenDoQueueProvider queue, IServiceProvider serviceProvider, IWhenDoJobExecutionManager jobExecutionManager,
            ILogger<WhenDoEngine> logger, IWhenDoRegistry registry, WhenDoConfiguration config, JobStorage hangfireStorage, IWhenDoJobManager jobManager)
        {
            this.queue = queue;
            this.logger = logger;
            this.serviceProvider = serviceProvider;
            this.registry = registry;
            this.config = config;
            this.hangfireStorage = hangfireStorage;
            this.jobExecutionManager = jobExecutionManager;
            this.jobManager = jobManager;

            RegisterExpressionProviders(); //TODO: make automatic registration configurable
        }

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            if (!config.ExternalHangfireServer)
            {
                try
                {
                    GlobalConfiguration.Configuration.UseActivator(new HangfireJobActivator(serviceProvider));
                    JobHelper.SetSerializerSettings(new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.All
                    });
                    this.hangfireServer = new BackgroundJobServer(hangfireStorage);
                }
                catch(Exception ex)
                {
                    logger.LogError("Error starting hangfire server", ex);
                }
            }
            
            logger.LogInformation("Start listing to queue");
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    while (queue.GetMessage(out IWhenDoMessage message))
                    {
                        logger.LogTrace("Message received", message);
                        await jobExecutionManager.HandleAsync(message);
                    }
                    await jobExecutionManager.HeartBeatAsync();
                    await Task.Delay(5000); //TODO: Replace by setting
                }
                catch(Exception ex)
                {
                    logger.LogCritical(ex, "Critical error in WhenDoEngine. Stopped running jobs.");
                    throw;
                }
            }
            if (hangfireServer != null)
            {
                hangfireServer.Dispose();
            }
        }

        private void RegisterExpressionProviders()
        {
            var providerInterface = typeof(IWhenDoExpressionProvider);

            var assembly = Assembly.GetEntryAssembly();
            var assemblies = new List<AssemblyName>(assembly.GetReferencedAssemblies());
            assemblies.Add(assembly.GetName());

            foreach (var assemblyName in assemblies)
            {
                assembly = Assembly.Load(assemblyName);

                foreach (var type in assembly.GetTypes().Where(x => providerInterface.IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract))
                {
                    string name;
                    if (type.Name.EndsWith("Provider"))
                        name = type.Name.Substring(0, type.Name.Length - 8);
                    else
                        name = type.Name;
                    registry.RegisterExpressionProvider(name, type);
                }
            }
        }

        public void RegisterCommandHandler<T>(string type) where T : class, IWhenDoCommandHandler
        {
            registry.RegisterCommandHandler<T>(type);
        }

        public Task RegisterJobAsync(JobDefinition jobDefinition)
        {
            return jobManager.RegisterJobAsync(jobDefinition);
        }

        public Task RegisterJobAsync(IWhenDoJob job)
        {
            return jobManager.RegisterJobAsync(job);
        }        

        public Task ClearJobsAsync()
        {
            return jobManager.ClearJobsAsync();
        }
    }
}
