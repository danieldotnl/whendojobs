using System;
using System.Collections.Generic;
using System.Text;
using WhenDoJobs.Core.Models;

namespace WhenDoJobs.Core.Interfaces
{
    public interface ICommandDefinition
    {
        Dictionary<string, object> Command { get; set; }
        ExecutionStrategy Execution { get; set; }

        ICommand Build();
    }
}
