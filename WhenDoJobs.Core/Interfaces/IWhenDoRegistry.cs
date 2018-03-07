using System;
using System.Collections.Generic;
using System.Text;

namespace WhenDoJobs.Core.Interfaces
{
    public interface IWhenDoRegistry
    {
        void RegisterCommandHandler<TCommand>(string type) where TCommand : class, IWhenDoCommandHandler;
        IWhenDoCommandHandler GetCommandHandler(string type);
        void RegisterConditionProvider(string name, Type type);
        Type GetConditionProviderType(string name);
        IWhenDoConditionProvider GetConditionProvider(string name);
    }
}
