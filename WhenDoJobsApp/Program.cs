using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Serilog;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using WhenDoJobsApp.Messages;
using WhenDoJobs.Core.Models;
using WhenDoJobs.Core;
using WhenDoJobs.Core.Interfaces;
using WhenDoJobs.InMemoryQueueProvider;

namespace WhenDoJobsApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                  .Enrich.FromLogContext()
                  .WriteTo.Console()
                  .MinimumLevel.Verbose()
                  .CreateLogger();

            IServiceProvider serviceProvider = ConfigureServices();

            var path = @"C:\temp\sample-conf.json";
            var newjob = JsonConvert.DeserializeObject<JobDefinition>(File.ReadAllText(path));

            var handler = serviceProvider.GetRequiredService<WhenDoEngine>();
            handler.RegisterJob(newjob);

            var ct = new CancellationToken();
            Task.Run(async () => await handler.RunAsync(ct));

            var queue = serviceProvider.GetRequiredService<IQueueProvider>();

            Console.ReadLine();
            queue.EnqueueMessage(new TemperatureMessage() { Temperature = 20.0D });

            Console.ReadLine();
        }

        private static IServiceProvider ConfigureServices()
        {
            IServiceCollection services = new ServiceCollection();
            services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog(dispose: true));
            services.AddSingleton<IQueueProvider, InMemoryQueueProvider>();
            services.AddWhenDoJob(x => x.UseInMemoryQueue()); //TODO: register in here the service providers           

            return services.BuildServiceProvider();
        }
    }
}