using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhenDoJobs.Core.Exceptions;
using WhenDoJobs.Core.Interfaces;
using WhenDoJobs.Core.Models;

namespace WhenDoJobs.Core.Services
{
    public class WhenDoJobManager : IWhenDoJobManager
    {
        private ILogger<WhenDoJobManager> logger;
        private IWhenDoRepository<IWhenDoJob> jobRepository;
        private IWhenDoRegistry registry;

        public WhenDoJobManager(ILogger<WhenDoJobManager> logger, IWhenDoRepository<IWhenDoJob> jobRepository, IWhenDoRegistry registry)
        {
            this.registry = registry;
            this.logger = logger;
            this.jobRepository = jobRepository;
        }

        public async Task ClearJobsAsync()
        {
            await jobRepository.RemoveAllAsync();
        }

        public async Task RegisterJobAsync(JobDefinition jobDefinition)
        {
            try
            {
                var job = CreateJobFromDefinition(jobDefinition);
                await RegisterJobAsync(job);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Could not register job {jobDefinition.Id}");
                throw;
            }
        }

        public async Task RegisterJobAsync(IWhenDoJob job)
        {
            try
            {
                if (job.Schedule != null)
                    job.SetNextRun(DateTimeOffset.Now); //TODO: should be provider by datetimeprovider?
                await jobRepository.SaveAsync(job);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Could not register job {job.Id}");
                throw;
            }
        }

        public virtual IWhenDoJob CreateJobFromDefinition(JobDefinition definition)
        {
            var providers = GetProviderInfoList(definition.Providers);

            var job = new WhenDoJob()
            {
                Id = definition.Id,
                Version = definition.Version,
                Disabled = definition.Disabled,
                DisabledFrom = definition.DisabledFrom,
                DisabledTill = definition.DisabledTill,
                ConditionProviders = providers,
                Schedule = definition.Schedule.ToWhenDoSchedule(),
                Type = (definition.Schedule == null) ? JobType.Message : JobType.Scheduled,
                Condition = WhenDoHelpers.ParseExpression<bool>(definition.When, providers),
                Commands = definition.Do.Select(x => CreateCommandFromDefinition(x, providers)).ToList()
            };
            return job;
        }

        public IWhenDoCommand CreateCommandFromDefinition(CommandDefinition definition, List<ExpressionProviderInfo> providerInfos)
        {
            ExecutionStrategy strategy = null;
            if (definition.Execution.Mode == ExecutionMode.Default || definition.Execution.Mode == ExecutionMode.Reliable)
                strategy = new ExecutionStrategy() { Mode = definition.Execution.Mode, Time = null };
            else
                strategy = CreateExecutionStrategyFromDefinition(definition.Execution, providerInfos);

            var command = new WhenDoCommand()
            {
                Id = Guid.NewGuid().ToString("N").ToLower(),
                Type = definition.Type,
                MethodName = definition.Command,
                Parameters = definition.Parameters,
                ExecutionStrategy = strategy
            };
            return command;
        }

        public ExecutionStrategy CreateExecutionStrategyFromDefinition(ExecutionStrategyDefinition definition, List<ExpressionProviderInfo> providerInfos)
        {
            return new ExecutionStrategy()
            {
                Mode = definition.Mode,
                Time = WhenDoHelpers.ParseExpression<TimeSpan>(definition.Time, providerInfos)
            };
        }

        private List<ExpressionProviderInfo> GetProviderInfoList(List<string> providerDefinitions)
        {
            var messageCount = 0;
            var providers = new List<ExpressionProviderInfo>();
            foreach (var prov in providerDefinitions)
            {
                var provider = new ExpressionProviderInfo();
                if (prov.Contains('='))
                {
                    var provPair = prov.Split('=');
                    provider.ShortName = provPair[0];
                    provider.FullName = provPair[1];
                    provider.ProviderType = registry.GetExpressionProviderType(provPair[1].Trim());
                }
                else
                {
                    provider.FullName = prov.Trim();
                    provider.ProviderType = registry.GetExpressionProviderType(prov.Trim());
                }
                if(typeof(IWhenDoMessage).IsAssignableFrom(provider.ProviderType))
                    messageCount++;
                providers.Add(provider);
            }
            if (messageCount > 1)
                throw new MoreThanOneMessageInConditionException();
            return providers;
        }
    }
}
