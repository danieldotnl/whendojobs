using System;
using System.Collections.Generic;
using System.Text;

namespace WhenDoJobs.Core.Models
{
    public class JobDefinition
    {
        public string Id { get; set; }
        public int Version { get; set; }
        public bool Disabled { get; set; }
        public TimeSpan? DisabledFrom { get; set; }
        public TimeSpan? DisabledTill { get; set; }
        public List<string> Providers { get; set; }
        public string When { get; set; }
        public List<ScheduleDefinition> Schedule { get; set; }

        public IEnumerable<CommandDefinition> Do { get; set; }
    }
}
