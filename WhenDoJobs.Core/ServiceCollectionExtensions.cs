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
        public static void AddWhenDoJob(this IServiceCollection services, Action<WhenDoOptions> setupAction)
        {
            if (services.Any(x => x.ServiceType == typeof(WhenDoOptions)))
                throw new InvalidOperationException("WhenDoJob services already registered");

            var options = new WhenDoOptions();
            setupAction?.Invoke(options);

            services.AddSingleton<WhenDoEngine>();
            services.AddTransient<IDateTimeProvider, DateTimeProvider>();
            services.AddTransient<IJob, Job<IMessageContext>>();
            services.AddTransient<IJobExecutor, JobExecutor>();
            services.AddSingleton<WhenDoOptions>(options);

        }

    }
}
