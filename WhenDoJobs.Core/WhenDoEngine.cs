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

        public WhenDoEngine(IWhenDoQueueProvider queue, IServiceProvider serviceProvider, IWhenDoJobManager jobManager,
            ILogger<WhenDoEngine> logger,IWhenDoRegistry registry, WhenDoConfiguration config, JobStorage hangfireStorage)
        {
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
                if (queue.GetMessage(out IWhenDoMessage message))
                {
                    logger.LogTrace("Message received", message);
                    await jobManager.Handle(message);
                    continue;
                }
                await Task.Delay(1000);
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

        public void RegisterJob(JobDefinition jobDefinition)
        {
            try
            {
                var providers = GetProviders(jobDefinition.Providers);
                //var type = registry.GetConditionProviderType(jobDefinition.Context);
                registry.RegisterJob(jobDefinition.ToJob(providers));
            }
            catch(Exception ex)
            {
                logger.LogError(ex, $"Could not register job {jobDefinition.Id}");
                throw;
            }
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

        public void ClearJobRegister()
        {
            registry.ClearJobRegister();
        }
    }
}
