using Hangfire;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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
        private IWhenDoQueueProvider persistence;
        private ILogger<WhenDoEngine> logger;
        private IServiceProvider serviceProvider;
        private IDateTimeProvider dateTimeProvider;
        private IWhenDoRegistry registry;
        private WhenDoConfiguration config;
        private BackgroundJobServer hangfireServer;
        private JobStorage hangfireStorage;

        public WhenDoEngine(IWhenDoQueueProvider persistence, IServiceProvider serviceProvider,
            ILogger<WhenDoEngine> logger, IDateTimeProvider dateTimeProvider, IWhenDoRegistry registry, WhenDoConfiguration config, JobStorage hangfireStorage)
        {
            this.persistence = persistence;
            this.logger = logger;
            this.serviceProvider = serviceProvider;
            this.dateTimeProvider = dateTimeProvider;
            this.registry = registry;
            this.config = config;
            this.hangfireStorage = hangfireStorage;

            RegisterMessageContexts();
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
            
            logger.LogInformation("Start listing to queue");
            while (!cancellationToken.IsCancellationRequested)
            {
                if (persistence.GetMessage(out IWhenDoMessageContext message))
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

        public void RegisterMessageContexts()
        {
            var contextMessage = typeof(IWhenDoMessageContext);

            var types = new List<Type>();
            var assembly = Assembly.GetEntryAssembly();
            var assemblies = new List<AssemblyName>(assembly.GetReferencedAssemblies());
            assemblies.Add(assembly.GetName());

            foreach (var assemblyName in assemblies)
            {
                assembly = Assembly.Load(assemblyName);

                foreach (var type in assembly.GetTypes().Where(x => contextMessage.IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract))
                {
                    registry.RegisterMessageContext(type.Name, type);
                    types.Add(type);
                }
            }
        }

        public async Task HandleMessage(IWhenDoMessageContext message)
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
            try
            {
                var job = CreateJob(jobDefinition);
                registry.RegisterJob(job);
            }
            catch(Exception ex)
            {
                logger.LogError(ex, $"Could not register job {jobDefinition.Id}");
                throw;
            }
        }

        public void RegisterCommandHandler<T>(string type) where T : class, IWhenDoCommandHandler
        {
            registry.RegisterCommandHandler<T>(type);
        }

        public void ClearJobRegister()
        {
            registry.ClearJobRegister();
        }

        private IWhenDoJob CreateJob(JobDefinition jobDefinition)
        {
            var type = registry.GetMessageContextType(jobDefinition.Context);
            var mi = typeof(WhenDoExtensions).GetMethod("ToJob", true);
            var method = mi.MakeGenericMethod(type);
            return (IWhenDoJob)method.Invoke(typeof(WhenDoExtensions), new object[] { jobDefinition });
        }
    }
}
