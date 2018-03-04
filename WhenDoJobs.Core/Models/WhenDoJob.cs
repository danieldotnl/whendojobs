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
        public List<string> ConditionProviders { get; set;} = new List<string>();
        public Delegate Condition { get; set; }
        public IEnumerable<IWhenDoCommand> Commands { get; set; }        
    }
}
