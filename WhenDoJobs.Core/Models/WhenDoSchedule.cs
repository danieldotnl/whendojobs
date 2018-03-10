using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WhenDoJobs.Core.Models
{

    //public class WhenDoSchedule
    //{
    //    public WhenDoDays ScheduledDays { get; set; }
    //    public List<TimeSpan> TimesOfDay { get; set; }

    //    public DateTimeOffset LastRun { get; set; }
    //    public DateTimeOffset NextRun { get; set; }
    //}        

    public class WhenDoSchedule
    {
        public Dictionary<DayOfWeek, List<TimeSpan>> Schedule { get; set; }
        public DateTimeOffset LastRun { get; set; }
        public DateTimeOffset NextRun { get; set; }
    }
}
