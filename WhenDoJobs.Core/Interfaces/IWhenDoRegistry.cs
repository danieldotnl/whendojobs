using System;
using System.Collections.Generic;
using System.Text;

namespace WhenDoJobs.Core.Interfaces
{
    public interface IWhenDoRegistry
    {
        List<IJob> Jobs { get; set; }

        void RegisterJob(IJob job);
        void RegisterCommandHandler<TCommand>(string type) where TCommand : class, ICommandHandler;
        ICommandHandler GetCommandHandler(string type);
        void BuildServiceProvider();
    }
}
