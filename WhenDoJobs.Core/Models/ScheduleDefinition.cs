using System;
using System.Collections.Generic;
using System.Text;

namespace WhenDoJobs.Core.Models
{
    public class ScheduleDefinition
    {
        public List<string> Days { get; set; }
        public List<string> TimesOfDay { get; set; }
    }
}
