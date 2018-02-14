using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using WhenDoJobs.Core.Interfaces;

namespace WhenDoJobs.Core.Models
{
    public class CommandDefinition
    {
        public string Type { get; set; }
        public string Command { get; set; }
        [JsonExtensionData]
        public Dictionary<string, object> Parameters { get; set; }
        public ExecutionStrategyDefinition Execution { get; set; } = new ExecutionStrategyDefinition();
    }
}
