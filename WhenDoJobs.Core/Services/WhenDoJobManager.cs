using Hangfire;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhenDoJobs.Core.Interfaces;
using WhenDoJobs.Core.Models;

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
                var jobs = await jobRepository.Get(x => x.Type == JobType.Message && IsRunnable(x, message));

                if (jobs.Any())
                    ExecuteJobOnThreadPool(jobs, message);
                else
                    logger.LogInformation("No jobs to be executed for message {message}", message);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error processing queue message: {error}", message);
            }
        }

        private void ExecuteJobOnThreadPool(IEnumerable<IWhenDoJob> jobs, IWhenDoMessage message)
        {
            try
            {
                Task.Run(() =>
                {
                    foreach (var job in jobs)
                    {
                        ExecuteJob(job, message);
                    }
                });

            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Something went wrong in Threadpool thread when executing jobs {String.Join(',', jobs.Select(x => x.Id))}");
            }
        }

        public async Task HeartBeatAsync()
        {
            var now = dtp.Now;
            var jobs = await jobRepository.Get(x => x.Type == JobType.Scheduled && IsRunnable(x, null) && x.ShouldRun(now));

            if (jobs.Any())
                ExecuteJobOnThreadPool(jobs, null);
        }

        public void ExecuteJob(IWhenDoJob job, IWhenDoMessage context)
        {
            if(job.Type == JobType.Scheduled)
                job.SetNextRun(dtp.Now);
            job.LastRun = dtp.Now;
            jobRepository.SaveAsync(job);

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
                            logger.LogInformation($"Set command {command.Type} for immediate execution");
                            break;
                        case ExecutionMode.Delayed:
                            hangfireClient.Schedule<IWhenDoCommandExecutor>(x => x.ExecuteAsync(context, job.Id, command.Id), command.ExecutionStrategy.Time);
                            logger.LogInformation($"Delayed command {command.Type} with {command.ExecutionStrategy.Time.ToString()}");
                            break;
                        case ExecutionMode.Scheduled:
                            var today = DateTimeOffset.Now.Date;
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
                if (message != null && cp.Equals(message.GetType().Name))
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
