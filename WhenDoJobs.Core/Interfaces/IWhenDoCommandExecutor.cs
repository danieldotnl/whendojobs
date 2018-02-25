using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace WhenDoJobs.Core.Interfaces
{
    public interface IWhenDoCommandExecutor
    {
        Task ExecuteAsync(IWhenDoMessageContext context, string type, string methodName, Dictionary<string, object> parameters);
    }
}
