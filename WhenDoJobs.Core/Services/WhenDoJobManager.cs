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
        private IWhenDoJobExecutor jobExecutor;


        public WhenDoJobManager(IDateTimeProvider dateTimeProvider, IWhenDoRegistry registry, ILogger<WhenDoJobManager> logger, IWhenDoJobExecutor jobExecutor)
        {
            this.dtp = dateTimeProvider;
            this.registry = registry;
            this.logger = logger;
            this.jobExecutor = jobExecutor;
        }

        public async Task Handle(IWhenDoMessage message)
        {
            try
            {
                var executableJobs = registry.Jobs.Where(x => IsRunnable(x, message)).ToList();

                if (executableJobs.Count() > 0)
                {
                    //TODO: changeserviceProvider.GetRequiredService<IWhenDoJobExecutor>();
                    //TODO: foreachasync extension method??
                    var tasks = new List<Task>();
                    foreach (var job in executableJobs)
                    {
                        tasks.Add(jobExecutor.ExecuteAsync(job, message));
                    }
                    await Task.WhenAll(tasks);
                }
                else
                    logger.LogInformation("No jobs to be executed for message {message}", message);
            }
            catch (Exception ex)
            {
                logger.LogError("Error processing queue message: {error}", ex, message);
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
