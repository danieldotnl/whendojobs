using System;
using System.Collections.Generic;
using System.Text;

namespace WhenDoJobs.Core.Interfaces
{
    public interface IWhenDoRegistry
    {
        IEnumerable<IWhenDoJob> Jobs { get; }

        void RegisterJob(IWhenDoJob job);
        void ClearJobRegister();
        void RegisterCommandHandler<TCommand>(string type) where TCommand : class, IWhenDoCommandHandler;
        IWhenDoCommandHandler GetCommandHandler(string type);
        void RegisterMessageContext(string name, Type type);
        Type GetMessageContextType(string name);
    }
}
