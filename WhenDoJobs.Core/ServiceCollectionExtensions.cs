using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WhenDoJobs.Core.Interfaces;
using WhenDoJobs.Core.Models;
using WhenDoJobs.Core.Services;

namespace WhenDoJobs.Core
{
    public static class ServiceCollectionExtensions
    {
        public static void AddWhenDoJob(this IServiceCollection services)
        {
            services.AddWhenDoJob(config => config.UseInMemoryQueue());
        }

        public static void AddWhenDoJob(this IServiceCollection services, Action<WhenDoConfiguration> setupAction)
        {
            if (services.Any(x => x.ServiceType == typeof(WhenDoConfiguration)))
                throw new InvalidOperationException("WhenDoJob services already registered");

            var config = new WhenDoConfiguration();
            setupAction?.Invoke(config);

            services.AddSingleton<IServiceCollection>(services);

            services.AddSingleton<IWhenDoEngine, WhenDoEngine>();
            services.AddTransient<IDateTimeProvider, DateTimeProvider>();
            services.AddTransient<IWhenDoExecutor, WhenDoExecutor>();
            services.AddSingleton<IWhenDoRegistry, WhenDoRegistry>();

            //register default handlers
            //var provider = services.BuildServiceProvider();
            //var engine = provider.GetRequiredService<IWhenDoEngine>();
            //engine.RegisterCommandHandler<LoggingCommandHandler>("Logging");
        }

        public static WhenDoConfiguration UseInMemoryQueue(this WhenDoConfiguration options)
        {
            options.UseQueue(sp => new InMemoryQueueProvider());
            return options;
        }
    }
}
