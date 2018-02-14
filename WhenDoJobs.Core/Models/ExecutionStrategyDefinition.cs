using System;
using System.Collections.Generic;
using System.Text;

namespace WhenDoJobs.Core.Models
{
    public class ExecutionStrategyDefinition
    {
        public ExecutionMode Mode { get; set; }
        public TimeSpan Time { get; set; }
    }
}
