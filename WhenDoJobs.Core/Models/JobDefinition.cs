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
        public string Context { get; set; }
        public string When { get; set; }

        public IEnumerable<CommandDefinition> Do { get; set; }
    }

    //public class JobDefinition
    //{
    //    public string Id { get; set; }
    //    public int Version { get; set; }
    //    public bool Disabled { get; set; }
    //    public TimeSpan? DisabledFrom { get; set; }
    //    public TimeSpan? DisabledTill { get; set; }
    //    public When When { get; set; }

    //    public IEnumerable<CommandDefinition> Do { get; set; }
    //}

    //public class When
    //{
    //    public WhenMessage Message { get; set; }
    //    public WhenSchedule Schedule { get; set; }
    //}

    //public class WhenMessage
    //{
    //    public List<string> Providers { get; set; }
    //    public string Context { get; set; }
    //    public string Condition { get; set; }
    //}

    //public class WhenSchedule
    //{
    //    public string Days { get; set; }
    //    public string[] TimesOfDay { get; set; }
    //}
}
