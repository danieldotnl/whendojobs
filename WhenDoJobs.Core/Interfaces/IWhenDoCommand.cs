using System;
using System.Collections.Generic;
using System.Text;
using WhenDoJobs.Core.Models;

namespace WhenDoJobs.Core.Interfaces
{
    public interface IWhenDoCommand
    {
        string Id { get; set; }
        string Type { get; set; }
        string MethodName { get; set; }
        Dictionary<string, object> Parameters { get; set; }
        ExecutionStrategy ExecutionStrategy { get; set; }

        object GetParameter(string name);
    }
}
