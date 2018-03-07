using Hangfire;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhenDoJobs.Core.Interfaces;

namespace WhenDoJobs.Core.Services
{
    public class WhenDoJobManager : IWhenDoJobManager
    {
        private IDateTimeProvider dtp;
        private IWhenDoRegistry registry;
        private ILogger<WhenDoJobManager> logger;
        private IWhenDoRepository<IWhenDoJob> jobRepository;
        private IBackgroundJobClient hangfireClient;

        public WhenDoJobManager(IDateTimeProvider dateTimeProvider, IWhenDoRegistry registry, 
            ILogger<WhenDoJobManager> logger, IWhenDoRepository<IWhenDoJob> jobRepository, IBackgroundJobClient hangfireClient)
        {
            this.hangfireClient = hangfireClient;
            this.dtp = dateTimeProvider;
            this.registry = registry;
            this.logger = logger;
            this.jobRepository = jobRepository;
        }

        public async Task HandleAsync(IWhenDoMessage message)
        {
            try
            {
                var jobs = await jobRepository.GetAllAsync();
                var executableJobs = jobs.Where(x => IsRunnable(x, message)).ToList();

                if (executableJobs.Count() > 0)
                {
                    //TODO: changeserviceProvider.GetRequiredService<IWhenDoJobExecutor>();
                    //TODO: foreachasync extension method??
                    executableJobs.ForEach(job => ExecuteJob(job, message));
                }
                else
                    logger.LogInformation("No jobs to be executed for message {message}", message);
            }
            catch (Exception ex)
            {
                logger.LogError("Error processing queue message: {error}", ex, message);
            }
        }

        public async Task HeartBeatAsync()
        {
            var time = dtp.Now;

            //select jobs with next run time > time - settings.heartbeatinterval * 2
            //check last run date

        }

        public void ExecuteJob(IWhenDoJob job, IWhenDoMessage context)
        {
            foreach (var command in job.Commands)
            {
                try
                {
                    switch (command.ExecutionStrategy.Mode)
                    {
                        //case ExecutionMode.Default:
                        //    var commandExecutor = serviceProvider.GetRequiredService<IWhenDoCommandExecutor>();
                        //    await commandExecutor.ExecuteAsync(context, command.Type, command.MethodName, command.Parameters);
                        //    break;

                        case ExecutionMode.Default:
                        case ExecutionMode.Reliable:
                            hangfireClient.Enqueue<IWhenDoCommandExecutor>(x => x.ExecuteAsync(context, job.Id, command.Id));
                            break;
                        case ExecutionMode.Delayed:
                            hangfireClient.Schedule<IWhenDoCommandExecutor>(x => x.ExecuteAsync(context, job.Id, command.Id), command.ExecutionStrategy.Time);
                            break;
                        case ExecutionMode.Scheduled:
                            var today = DateTime.Today;
                            var time = command.ExecutionStrategy.Time;
                            var executionTime = (today + time > DateTime.Now) ? today + time : today.AddDays(1) + time;
                            hangfireClient.Schedule<IWhenDoCommandExecutor>(x => x.ExecuteAsync(context, job.Id, command.Id), executionTime);
                            logger.LogInformation($"Scheduled command {command.Type} at {time.ToString()}");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, $"Error when executing command {command.Id}: {ex.Message}");
                }
            }
        }

        public bool IsRunnable(IWhenDoJob job, IWhenDoMessage message)
        {
            if (job.Disabled)
                return false;
            if (job.DisabledFrom.HasValue && job.DisabledTill.HasValue)
            {
                var time = dtp.CurrentTime;
                if (time > job.DisabledFrom.Value && time < job.DisabledTill.Value)
                    return false;
            }

            var providers = CollectConditionProviders(job.ConditionProviders, message);

            if (!(bool)job.Condition.DynamicInvoke(providers.ToArray()))
                return false;
            return true;
        }

        private List<IWhenDoConditionProvider> CollectConditionProviders(List<string> conditionProviders, IWhenDoMessage message)
        {
            var providers = new List<IWhenDoConditionProvider>();
            foreach (var cp in conditionProviders)
            {
                if (cp.Equals(message.GetType().Name))
                    providers.Add(message);
                else
                {
                    providers.Add(registry.GetConditionProvider(cp));
                }

            }
            return providers;
        }
    }
}
