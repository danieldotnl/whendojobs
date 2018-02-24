using System;
using System.Collections.Generic;
using WhenDoJobs.Core.Models;

namespace WhenDoJobs.Core.Interfaces
{
    public interface IWhenDoJob
    {
        string Id { get; set; }
        int Version { get; set; }
        TimeSpan? DisabledFrom { get; set; }
        TimeSpan? DisabledTill { get; set; }
        bool Disabled { get; set; }
        Delegate Condition { get; set; }
        IEnumerable<IWhenDoCommand> Commands { get; set; }

        bool Evaluate(IWhenDoMessageContext context);
        bool IsRunnable(IDateTimeProvider dateTimeProvider);
    }
}
