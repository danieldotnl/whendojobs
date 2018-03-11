using System;
using System.Collections.Generic;
using System.Text;

namespace WhenDoJobs.Core.Interfaces
{
    public interface IWhenDoRegistry
    {
        void RegisterCommandHandler<TCommand>(string type) where TCommand : class, IWhenDoCommandHandler;
        IWhenDoCommandHandler GetCommandHandler(string type);
        void RegisterExpressionProvider(string name, Type type);
        Type GetExpressionProviderType(string name);
        IWhenDoExpressionProvider GetExpressionProvider(string name);
    }
}
