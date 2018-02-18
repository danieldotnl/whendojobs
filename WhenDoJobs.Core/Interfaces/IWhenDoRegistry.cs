using System;
using System.Collections.Generic;
using System.Text;

namespace WhenDoJobs.Core.Interfaces
{
    public interface IWhenDoRegistry
    {
        List<IWhenDoJob> Jobs { get; set; }

        void RegisterJob(IWhenDoJob job);
        void RegisterCommandHandler<TCommand>(string type) where TCommand : class, IWhenDoCommandHandler;
        IWhenDoCommandHandler GetCommandHandler(string type);
        void BuildServiceProvider();
    }
}
