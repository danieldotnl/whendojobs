using System;
using System.Collections.Generic;
using System.Text;
using WhenDoJobs.Core.Interfaces;

namespace WhenDoJobs.Core.Models
{
    public class Command : ICommand
    {
        public string Type { get; set; }
        public string MethodName { get; set; }
        public Dictionary<string, object> Parameters { get; set; } = new Dictionary<string, object>();
        public ExecutionStrategy ExecutionStrategy { get; set; }

        public object GetParameter(string name)
        {
            return Parameters[name];
        }
    }
}
