using System;
using System.Collections.Generic;
using System.Text;

namespace WhenDoJobs.Core.Models
{
    public class ExecutionStrategy
    {
        public ExecutionMode Mode { get; set; }
        public Delegate Time { get; set; }
    }
}
