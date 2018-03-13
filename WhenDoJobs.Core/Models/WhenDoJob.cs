using System;
using System.Collections.Generic;
using System.Linq;
using WhenDoJobs.Core.Interfaces;

namespace WhenDoJobs.Core.Models
{
    public enum JobType { Message, Scheduled }

    public class WhenDoJob : IWhenDoJob
    {
        public string Id { get; set; }
        public JobType Type { get; set; }
        public int Version { get; set; }
        public bool Disabled { get; set; }
        public TimeSpan? DisabledFrom { get; set; }
        public TimeSpan? DisabledTill { get; set; }
        public List<ExpressionProviderInfo> ConditionProviders { get; set; } = new List<ExpressionProviderInfo>();
        public Delegate Condition { get; set; }
        public Dictionary<DayOfWeek, List<TimeSpan>> Schedule { get; set; }
        public IEnumerable<IWhenDoCommand> Commands { get; set; }

        public DateTimeOffset? LastRun { get; set; }
        public DateTimeOffset NextRun { get; set; }

        public bool ShouldRun(DateTimeOffset refTime)
        {
            return NextRun < refTime;
        }

        public void SetNextRun(DateTimeOffset refDatetime)
        {
            var count = 0;
            while (true)
            {
                var refDay = refDatetime.DayOfWeek;
                if (Schedule.ContainsKey(refDay))
                {
                    var time = Schedule[refDay].Distinct().OrderBy(x => x).Where(x => x > refDatetime.TimeOfDay);
                    if (time.Any())
                    {
                        NextRun = refDatetime.Date + time.First();
                        break;
                    }
                }
                refDatetime = refDatetime.Date.AddDays(1);
                count++;
                if (count > 6)
                    throw new ArgumentOutOfRangeException($"Cannot determine next run datetime within 7 days");
            }
        }
    }
}

