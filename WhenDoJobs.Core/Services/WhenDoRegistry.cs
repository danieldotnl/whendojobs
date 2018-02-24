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
        private IServiceProvider provider;
        private Dictionary<string, Type> commandHandlers = new Dictionary<string, Type>();
        private Dictionary<string, Type> messageContexts = new Dictionary<string, Type>();
        private List<IWhenDoJob> jobs = new List<IWhenDoJob>();

        public IEnumerable<IWhenDoJob> Jobs { get { return jobs; } }

        public WhenDoRegistry(IServiceCollection services, IServiceProvider serviceProvider)
        {
            this.services = services;
            this.provider = serviceProvider;
        }

        public IWhenDoCommandHandler GetCommandHandler(string type)
        {
            if (commandHandlers.ContainsKey(type))
            {
                var cmd = commandHandlers[type];
                var handler = provider.GetRequiredService(cmd);
                return (IWhenDoCommandHandler)handler;
            }
            else
                throw new CommandHandlerNotRegisteredException("No such command handler has been registered", type);
        }

        public void RegisterCommandHandler<T>(string type) where T : class, IWhenDoCommandHandler
        {
            services.AddTransient<T>();
            provider = services.BuildServiceProvider();
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

        public void RegisterMessageContext(string name, Type type)
        {
            messageContexts.Add(name, type);
        }

        public Type GetMessageContextType(string name)
        {
            if (messageContexts.ContainsKey(name))
            {
                return messageContexts[name];
            }
            else
                throw new MessageContextNotRegisteredException("No such message context has been registered", name);
        }
    }
}
