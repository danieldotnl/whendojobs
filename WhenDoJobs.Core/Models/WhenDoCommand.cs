using System;
using System.Collections.Generic;
using System.Text;
using WhenDoJobs.Core.Interfaces;

namespace WhenDoJobs.Core.Models
{
    public class WhenDoCommand : IWhenDoCommand
    {
        public string Type { get; set; }
        public string MethodName { get; set; }
        public Dictionary<string, object> Parameters { get; set; } = new Dictionary<string, object>();
        public ExecutionStrategy ExecutionStrategy { get; set; }

        public WhenDoCommand()
        {
            ExecutionStrategy = new ExecutionStrategy();
        }

        public object GetParameter(string name)
        {
            return Parameters[name];
        }
    }
}
