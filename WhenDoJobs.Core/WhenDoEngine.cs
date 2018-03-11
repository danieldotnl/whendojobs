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
        private IWhenDoJobManager jobManager;
        private IWhenDoRepository<IWhenDoJob> jobRepository;

        public WhenDoEngine(IWhenDoQueueProvider queue, IServiceProvider serviceProvider, IWhenDoJobManager jobManager, IWhenDoRepository<IWhenDoJob> jobRepository,
            ILogger<WhenDoEngine> logger, IWhenDoRegistry registry, WhenDoConfiguration config, JobStorage hangfireStorage)
        {
            this.jobRepository = jobRepository;
            this.queue = queue;
            this.logger = logger;
            this.serviceProvider = serviceProvider;
            this.registry = registry;
            this.config = config;
            this.hangfireStorage = hangfireStorage;
            this.jobManager = jobManager;

            RegisterConditionProviders();
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
                        await jobManager.HandleAsync(message);
                    }
                    await jobManager.HeartBeatAsync();
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

        private void RegisterConditionProviders()
        {
            var providerInterface = typeof(IWhenDoConditionProvider);

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
                    registry.RegisterConditionProvider(name, type);
                }
            }
        }

        public async Task RegisterJobAsync(JobDefinition jobDefinition)
        {
            try
            {
                var job = CreateJobFromDefinition(jobDefinition);
                await RegisterJobAsync(job);
            }
            catch(Exception ex)
            {
                logger.LogError(ex, $"Could not register job {jobDefinition.Id}");
                throw;
            }
        }

        public async Task RegisterJobAsync(IWhenDoJob job)
        {
            try
            {
                if(job.Schedule != null)
                    job.SetNextRun(DateTimeOffset.Now);
                await jobRepository.SaveAsync(job);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Could not register job {job.Id}");
                throw;
            }
        }

        public virtual IWhenDoJob CreateJobFromDefinition(JobDefinition definition)
        {
            var providers = GetProviders(definition.Providers);
            
            var job = new WhenDoJob()
            {
                Id = definition.Id,
                Version = definition.Version,
                Disabled = definition.Disabled,
                DisabledFrom = definition.DisabledFrom,
                DisabledTill = definition.DisabledTill,
                ConditionProviders = GetProviderNames(definition.Providers),
                Schedule =  definition.Schedule.ToWhenDoSchedule(),
                Type = (definition.Schedule == null) ? JobType.Message : JobType.Scheduled,
                Condition = WhenDoHelpers.ParseExpression(definition.When, providers),
                Commands = definition.Do.Select(x => x.ToCommand()).ToList()
            };
            return job;
        }        

        private static List<string> GetProviderNames(List<string> providerDefs)
        {
            var names = new List<string>();
            foreach (var prov in providerDefs)
            {
                if (prov.Contains('='))
                    names.Add(prov.Split('=')[1]);
                else
                    names.Add(prov);
            }
            return names;
        }

        private Dictionary<string, Type> GetProviders(List<string> providerList)
        {
            Dictionary<string, Type> providers = new Dictionary<string, Type>();
            foreach (var prov in providerList)
            {
                if (prov.Contains('='))
                {
                    var provPair = prov.Split('=');
                    providers.Add(provPair[0].Trim(), registry.GetConditionProviderType(provPair[1].Trim()));
                }
                else
                {
                    providers.Add(prov, registry.GetConditionProviderType(prov.Trim()));
                }
            }
            return providers;
        }

        public void RegisterCommandHandler<T>(string type) where T : class, IWhenDoCommandHandler
        {
            registry.RegisterCommandHandler<T>(type);
        }

        public async Task ClearJobsAsync()
        {
            await jobRepository.RemoveAllAsync();
        }
    }
}
