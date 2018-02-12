using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using WhenDoJobs.Core.Interfaces;

namespace WhenDoJobs.Core.Models
{
    public class CommandDefinition : ICommandDefinition
    {
        [JsonExtensionData]
        public Dictionary<string, object> Command { get; set; }
        public ExecutionStrategy Execution { get; set; }

        public ICommand Build()
        {
            throw new NotImplementedException();
        }
    }
}
