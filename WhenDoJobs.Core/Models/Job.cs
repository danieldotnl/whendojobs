using System;
using System.Collections.Generic;
using WhenDoJobs.Core.Interfaces;

namespace WhenDoJobs.Core.Models
{
    public class Job<TContext> : IJob where TContext : IMessageContext
    {
        private IDateTimeProvider dtp;

        public string Id { get; set; }
        public int Version { get; set; }
        public bool Disabled { get; set; }
        public TimeSpan? DisabledFrom { get; set; }
        public TimeSpan? DisabledTill { get; set; }
        public Delegate Condition { get; set; }
        public IEnumerable<CommandDefinition> CommandDefinitions { get; set; }

        public Job(IDateTimeProvider dateTimeProvider)
        {
            this.dtp = dateTimeProvider;
        }

        public bool IsRunnable()
        {
            if (Disabled)
                return false;
            if (DisabledFrom.HasValue && DisabledTill.HasValue)
            {
                return dtp.CurrentTime < DisabledFrom.Value || dtp.CurrentTime > DisabledTill.Value;
            }
            return true;
        }

        public bool Evaluate(IMessageContext context)
        {
            var result = (bool)Condition.DynamicInvoke(context);
            return result;
        }
    }
}
