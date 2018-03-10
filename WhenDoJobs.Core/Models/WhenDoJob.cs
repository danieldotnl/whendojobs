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
        public List<string> ConditionProviders { get; set; } = new List<string>();
        public Delegate Condition { get; set; }
        public Dictionary<DayOfWeek, List<TimeSpan>> Schedule { get; set; }
        public IEnumerable<IWhenDoCommand> Commands { get; set; }

        public DateTimeOffset LastRun { get; set; }
        public DateTimeOffset NextRun { get; set; }

        public bool ShouldRun(DateTimeOffset refTime)
        {
            return NextRun < refTime;
        }



        //public DateTimeOffset GetNextScheduledDate(DateTimeOffset refDate)
        //{
        //    while (!IsScheduled(refDate.DayOfWeek.ToWhenDoDays()))
        //    {
        //        refDate.AddDays(1);
        //    }
        //    return refDate.Date;
        //}

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

            ///*
            // * check if nextrun > refdatetime, if so return
            // * Check if date's day is part of scheduled days
            // * yes: find any time after current time
            // * found: set nextrun and stop
            // * notfound: set refdatetime to next day
            // */

            //if ((refDatetime.Date - DateTimeOffset.Now).Days > 7)
            //    throw new ArgumentOutOfRangeException("Cannot determine next run datetime");

            //if (NextRun > refDatetime)
            //    return;

            //if (IsScheduled(refDatetime.DayOfWeek.ToWhenDoDays()))
            //{
            //    foreach (var time in TimesOfDay.Where(x => x > refDatetime.TimeOfDay).OrderBy(x => x))
            //    {
            //        if (time > refDatetime.TimeOfDay)
            //        {
            //            NextRun = refDatetime.Date + time;
            //            return;
            //        }
            //    }
            //}
            //SetNextRun(refDatetime.Date.AddDays(1));
        }
    }
}

