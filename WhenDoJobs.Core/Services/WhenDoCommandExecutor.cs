using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhenDoJobs.Core.Exceptions;
using WhenDoJobs.Core.Interfaces;

namespace WhenDoJobs.Core.Services
{
    public class WhenDoCommandExecutor : IWhenDoCommandExecutor
    {
        private IWhenDoRegistry registry;
        private ILogger<WhenDoCommandExecutor> logger;

        public WhenDoCommandExecutor(IWhenDoRegistry registry, ILogger<WhenDoCommandExecutor> logger)
        {
            this.registry = registry;
            this.logger = logger;
        }

        public async Task ExecuteAsync(string type, string methodName, Dictionary<string, object> parameters)
        {
            var commandHandler = registry.GetCommandHandler(type);
            try
            {
                var method = commandHandler.GetType().GetMethods()
                    .Where(m => m.Name.Equals(methodName))
                    .Where(n => n.GetParameters().Count() == parameters.Count)
                    .FirstOrDefault();

                if (method == null)
                    throw new InvalidCommandException($"Could not find method {methodName} in type {type} with {parameters.Count} parameters", parameters.Keys);
                var methodParams = method.GetParameters().ToList();

                var invocationParams = new List<object>();
                methodParams.ForEach(x => invocationParams.Add(parameters[x.Name]));
                await (Task)method.Invoke(commandHandler, invocationParams.ToArray());

                logger.LogTrace($"Succesfully executed {type}");
            }
            catch (KeyNotFoundException ex)
            {
                throw new InvalidCommandException("Invalid command, could not find required parameters", parameters.Keys, ex);
            }
        }
    }
}
