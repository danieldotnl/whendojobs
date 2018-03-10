using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Serilog;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using WhenDoJobs.Core.Models;
using WhenDoJobs.Core;
using WhenDoJobs.Core.Interfaces;
using WhenDoJobs.Core.Services;

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
            var engine = serviceProvider.GetRequiredService<IWhenDoEngine>();


            var file = @".\message-job.json";
            var job = JsonConvert.DeserializeObject<JobDefinition>(File.ReadAllText(file));
            engine.RegisterJobAsync(job);


            var file2 = @".\recurring-job.json";
            var job2 = JsonConvert.DeserializeObject<JobDefinition>(File.ReadAllText(file2));
            engine.RegisterJobAsync(job2);
            
            engine.RegisterCommandHandler<LoggingCommandHandler>("Logging");

            var ct = new CancellationToken();
            Task.Run(async () => await engine.RunAsync(ct));

            var queue = serviceProvider.GetRequiredService<IWhenDoQueueProvider>();

            Console.ReadLine();
            queue.EnqueueMessage(new TemperatureMessage() { Temperature = 19.5D, Area = "Livingroom" });

            Console.ReadLine();
        }

        private static IServiceProvider ConfigureServices()
        {
            IServiceCollection services = new ServiceCollection();
            services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog(dispose: true));
            services.AddWhenDoJob();       

            return services.BuildServiceProvider();
        }
    }
}