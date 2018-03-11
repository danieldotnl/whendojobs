using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using WhenDoJobs.Core.Exceptions;
using WhenDoJobs.Core.Interfaces;
using WhenDoJobs.Core.Models;

namespace WhenDoJobs.Core.Services
{
    public class WhenDoRegistry : IWhenDoRegistry
    {
        private IServiceCollection services;
        private IServiceProvider serviceProvider;
        private Dictionary<string, Type> commandHandlers = new Dictionary<string, Type>();
        private Dictionary<string, Type> expressionProviders = new Dictionary<string, Type>();

        public WhenDoRegistry(IServiceCollection services, IServiceProvider serviceProvider)
        {
            this.services = services;
            this.serviceProvider = serviceProvider;
        }

        public IWhenDoCommandHandler GetCommandHandler(string type)
        {
            if (commandHandlers.ContainsKey(type))
            {
                var cmd = commandHandlers[type];
                var handler = serviceProvider.GetRequiredService(cmd);
                return (IWhenDoCommandHandler)handler;
            }
            else
                throw new CommandHandlerNotRegisteredException("No such command handler has been registered", type);
        }

        public void RegisterCommandHandler<T>(string type) where T : class, IWhenDoCommandHandler
        {
            services.AddTransient<T>();
            serviceProvider = services.BuildServiceProvider();
            commandHandlers.Add(type, typeof(T));
        }

        public void RegisterExpressionProvider(string name, Type type)
        {
            services.AddTransient(type);
            serviceProvider = services.BuildServiceProvider();
            expressionProviders.Add(name, type);
        }

        public Type GetExpressionProviderType(string name)
        {
            if (expressionProviders.ContainsKey(name))
            {
                return expressionProviders[name];
            }
            else
                throw new ExpressionProviderNotRegisteredException("No such expression provider has been registered", name);
        }

        public IWhenDoExpressionProvider GetExpressionProvider(string name)
        {
            if (expressionProviders.ContainsKey(name))
                return (IWhenDoExpressionProvider)serviceProvider.GetRequiredService(expressionProviders[name]);
            else
                throw new ExpressionProviderNotRegisteredException("No such expression provider has been registered", name);
        }
    }
}
