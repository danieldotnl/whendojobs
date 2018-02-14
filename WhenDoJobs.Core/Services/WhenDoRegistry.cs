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
        public List<IJob> Jobs { get; set; } = new List<IJob>();

        public WhenDoRegistry(IServiceCollection services, IServiceProvider serviceProvider)
        {
            this.services = services;
            this.provider = serviceProvider;
        }

        public void BuildServiceProvider()
        {
            provider = services.BuildServiceProvider();
        }

        public ICommandHandler GetCommandHandler(string type)
        {
            var cmd = commands[type];
            if (cmd == null)
                throw new CommandHandlerNotRegisteredException("No such command handler has been registered", type);
            return (ICommandHandler) provider.GetRequiredService(cmd);
        }

        public void RegisterCommandHandler<TCommand>(string type) where TCommand : class, ICommandHandler
        {
            services.AddTransient<TCommand>();
            commands.Add(type, typeof(TCommand));
        }

        public void RegisterJob(IJob job)
        {
            this.Jobs.Add(job);
        }

    }
}
