using System;
using System.Collections.Generic;
using WhenDoJobs.Core.Models;

namespace WhenDoJobs.Core.Interfaces
{
    public interface IJob
    {
        string Id { get; set; }
        int Version { get; set; }
        TimeSpan? DisabledFrom { get; set; }
        TimeSpan? DisabledTill { get; set; }
        bool Disabled { get; set; }
        Delegate Condition { get; set; }
        IEnumerable<CommandDefinition> CommandDefinitions { get; set; }

        bool Evaluate(IMessageContext context);
        bool IsRunnable();
    }
}
