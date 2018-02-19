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
        private Dictionary<string, Type> commands = new Dictionary<string, Type>();
        private List<IWhenDoJob> jobs = new List<IWhenDoJob>();

        public IEnumerable<IWhenDoJob> Jobs { get { return jobs; } }

        public WhenDoRegistry(IServiceCollection services, IServiceProvider serviceProvider)
        {
            this.services = services;
            this.provider = serviceProvider;
        }

        public void BuildServiceProvider()
        {
            provider = services.BuildServiceProvider();
        }

        public IWhenDoCommandHandler GetCommandHandler(string type)
        {
            if (commands.ContainsKey(type))
            {
                var cmd = commands[type];
                return (IWhenDoCommandHandler)provider.GetRequiredService(cmd);
            }
            else
                throw new CommandHandlerNotRegisteredException("No such command handler has been registered", type);
        }

        public void RegisterCommandHandler<TCommand>(string type) where TCommand : class, IWhenDoCommandHandler
        {
            services.AddTransient<TCommand>();
            commands.Add(type, typeof(TCommand));
        }

        public void RegisterJob(IWhenDoJob job)
        {
            jobs.Add(job);
        }

        public void ClearJobRegister()
        {
            jobs.Clear();
        }

    }
}
