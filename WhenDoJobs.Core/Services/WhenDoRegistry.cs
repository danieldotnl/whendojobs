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
        private Dictionary<string, Type> conditionProviders = new Dictionary<string, Type>();
        private List<IWhenDoJob> jobs = new List<IWhenDoJob>();

        public IEnumerable<IWhenDoJob> Jobs { get { return jobs; } }

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

        public void RegisterJob(IWhenDoJob job)
        {
            jobs.Add(job);
        }

        public void ClearJobRegister()
        {
            jobs.Clear();
        }

        public void RegisterConditionProvider(string name, Type type)
        {
            services.AddTransient(type);
            serviceProvider = services.BuildServiceProvider();
            conditionProviders.Add(name, type);
        }

        public Type GetConditionProviderType(string name)
        {
            if (conditionProviders.ContainsKey(name))
            {
                return conditionProviders[name];
            }
            else
                throw new ConditionProviderNotRegisteredException("No such condition provider has been registered", name);
        }

        public IWhenDoConditionProvider GetConditionProvider(string name)
        {
            if (conditionProviders.ContainsKey(name))
                return (IWhenDoConditionProvider)serviceProvider.GetRequiredService(conditionProviders[name]);
            else
                throw new ConditionProviderNotRegisteredException("No such condition provider has been registered", name);
        }
    }
}
