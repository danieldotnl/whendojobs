﻿using Hangfire;
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
        
        public static void AddWhenDoJob(this IServiceCollection services, Action<WhenDoConfiguration> setupAction = null)
        {
            if (services.Any(x => x.ServiceType == typeof(WhenDoConfiguration)))
                throw new InvalidOperationException("WhenDoJob services already registered");

            var config = new WhenDoConfiguration();
            setupAction?.Invoke(config);

            services.AddSingleton<IServiceCollection>(services);

            services.AddTransient<JobStorage>(config.HangfireStorageFactory);
            services.AddSingleton<WhenDoConfiguration>(config);
            services.AddSingleton<IQueueProvider>(config.QueueFactory);
            services.AddTransient<IBackgroundJobClient, BackgroundJobClient>();
            services.AddSingleton<IWhenDoEngine, WhenDoEngine>();
            services.AddTransient<IDateTimeProvider, DateTimeProvider>();
            services.AddTransient<IWhenDoJobExecutor, WhenDoJobExecutor>();
            services.AddTransient<IWhenDoCommandExecutor, WhenDoCommandExecutor>();
            services.AddSingleton<IWhenDoRegistry, WhenDoRegistry>();

            //JobStorage.Current = config.HangfireStorageFactory()

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
