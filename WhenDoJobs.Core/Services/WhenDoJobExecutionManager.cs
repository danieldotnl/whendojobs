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
    public class WhenDoJobExecutionManager : IWhenDoJobExecutionManager
    {
        private IDateTimeProvider dtp;
        private IWhenDoRegistry registry;
        private ILogger<WhenDoJobExecutionManager> logger;
        private IWhenDoRepository<IWhenDoJob> jobRepository;
        private IBackgroundJobClient hangfireClient;

        public WhenDoJobExecutionManager(IDateTimeProvider dateTimeProvider, IWhenDoRegistry registry,
            ILogger<WhenDoJobExecutionManager> logger, IWhenDoRepository<IWhenDoJob> jobRepository, IBackgroundJobClient hangfireClient)
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
                var jobs = await jobRepository.GetAsync(x => x.Type == JobType.Message && IsRunnable(x, message));
                
                if (jobs.Any())
                {
                    foreach (var job in jobs)
                    {
                        ExecuteJob(job, message);
                    }
                }
                else
                    logger.LogInformation("No jobs to be executed for message {message}", message);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error processing queue message: {error}", message);
            }
        }

        public async Task HeartBeatAsync()
        {
            var now = dtp.Now;
            var jobs = await jobRepository.GetAsync(x => x.Type == JobType.Scheduled && IsRunnable(x, null) && x.ShouldRun(now));

            if (jobs.Any())
            {
                foreach (var job in jobs)
                {
                    ExecuteJob(job, null);
                }
            }
        }

        public void ExecuteJob(IWhenDoJob job, IWhenDoMessage context)
        {
            if (job.Commands.Count() == 0)
            {
                logger.LogWarning("Will not execute job {id} as it does not contain any commands", job.Id);
                return;
            }

            if (job.Type == JobType.Scheduled)
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
                            {
                                hangfireClient.Enqueue<IWhenDoCommandExecutor>(x => x.ExecuteAsync(context, job.Id, command.Id));
                                logger.LogInformation($"Set command {command.Type} for immediate execution");
                            }
                            break;
                        case ExecutionMode.Delayed:
                            {
                                var providers = GetExpressionProviderInstancesForDelegate(command.ExecutionStrategy.Time, null).ToArray();
                                var time = (TimeSpan)command.ExecutionStrategy.Time.DynamicInvoke(providers);
                                hangfireClient.Schedule<IWhenDoCommandExecutor>(x => x.ExecuteAsync(context, job.Id, command.Id), time);
                                logger.LogInformation($"Delayed command {command.Type} with {time.ToString()}");
                            }
                            break;
                        case ExecutionMode.Scheduled:
                            {
                                var providers = GetExpressionProviderInstancesForDelegate(command.ExecutionStrategy.Time, null).ToArray();

                                var today = DateTimeOffset.Now.Date;
                                var time = (TimeSpan)command.ExecutionStrategy.Time.DynamicInvoke(providers);
                                var executionTime = (today + time > DateTime.Now) ? today + time : today.AddDays(1) + time;
                                hangfireClient.Schedule<IWhenDoCommandExecutor>(x => x.ExecuteAsync(context, job.Id, command.Id), executionTime);
                                logger.LogInformation($"Scheduled command {command.Type} at {executionTime.ToString()}");
                            }
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

            if (message != null && !job.Condition.RequiresMessage(message))
                return false;

            var providers = GetExpressionProviderInstancesForDelegate(job.Condition, message);

            if (!(bool)job.Condition.DynamicInvoke(providers.ToArray()))
                return false;
            return true;
        }

        private List<IWhenDoExpressionProvider> GetExpressionProviderInstancesForDelegate(Delegate del, IWhenDoMessage message)
        {
            var providers = new List<IWhenDoExpressionProvider>();
            var fullNames = del.ExtractProviderFullNames();

            foreach (var name in fullNames)
            {
                if (message != null && name.Equals(message.GetType().Name))
                    providers.Add(message);
                else
                {
                    var instance = registry.GetExpressionProviderInstance(name);
                    if (instance != null)
                        providers.Add(instance);
                }
            }
            return providers;
        }
    }
}
