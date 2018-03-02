using System;
using System.Collections.Generic;
using WhenDoJobs.Core.Interfaces;

namespace WhenDoJobs.Core.Models
{
    public class WhenDoJob : IWhenDoJob
    {
        public string Id { get; set; }
        public int Version { get; set; }
        public bool Disabled { get; set; }
        public TimeSpan? DisabledFrom { get; set; }
        public TimeSpan? DisabledTill { get; set; }
        public Delegate Condition { get; set; }
        public IEnumerable<IWhenDoCommand> Commands { get; set; }

        public bool IsRunnable(IDateTimeProvider dtp)
        {
            if (Disabled)
                return false;
            if (DisabledFrom.HasValue && DisabledTill.HasValue)
            {
                var time = dtp.CurrentTime;
                return time < DisabledFrom.Value || time > DisabledTill.Value;
            }
            return true;
        }

        public bool Evaluate(IWhenDoMessageContext context)
        {
            var result = (bool)Condition.DynamicInvoke(context);
            return result;
        }
    }
}
