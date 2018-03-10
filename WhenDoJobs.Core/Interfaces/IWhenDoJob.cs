using System;
using System.Collections.Generic;
using WhenDoJobs.Core.Models;

namespace WhenDoJobs.Core.Interfaces
{
    public interface IWhenDoJob
    {
        string Id { get; set; }
        JobType Type { get; set; }
        int Version { get; set; }
        bool Disabled { get; set; }
        TimeSpan? DisabledFrom { get; set; }
        TimeSpan? DisabledTill { get; set; }
        List<string> ConditionProviders { get; set; }
        Delegate Condition { get; set; }
        Dictionary<DayOfWeek, List<TimeSpan>> Schedule { get; set; }
        IEnumerable<IWhenDoCommand> Commands { get; set; }

        DateTimeOffset LastRun { get; set; }
        DateTimeOffset NextRun { get; set; }

        bool ShouldRun(DateTimeOffset refTime);
        void SetNextRun(DateTimeOffset refDatetime);
    }
}
