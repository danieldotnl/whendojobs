using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
        private IWhenDoRepository<IWhenDoJob> repository;

        public WhenDoCommandExecutor(IWhenDoRegistry registry, IWhenDoRepository<IWhenDoJob> repository, ILogger<WhenDoCommandExecutor> logger)
        {
            this.repository = repository;
            this.registry = registry;
            this.logger = logger;
        }

        public async Task ExecuteAsync(IWhenDoMessage context, string jobId, string commandId)
        {
            var job = await repository.GetByIdAsync(jobId);
            if (job == null)
                throw new ArgumentException($"Job {jobId} not found in repository.");
            var command = job.Commands.Where(x => x.Id == commandId).FirstOrDefault();
            if (command == null)
                throw new ArgumentException($"Job {jobId} does not contain command {commandId}");

            var commandHandler = registry.GetCommandHandler(command.Type);

            try
            {
                var method = FindMethod(commandHandler, command.MethodName, command.Parameters == null ? 0 : command.Parameters.Count);

                if (method == null)
                    throw new InvalidCommandException($"Could not find method {command.MethodName} in type {command.Type}"
                         + " with {command.Parameters.Count} parameters", command.Parameters.Keys);

                var invocationParams = new List<object>();
                var methodParams = method.GetParameters();

                foreach (var param in methodParams)
                {
                    if (param.ParameterType.Equals(typeof(IWhenDoMessage)))
                        invocationParams.Add(context);
                    else
                        invocationParams.Add(command.Parameters[param.Name]);
                }

                await (Task)method.Invoke(commandHandler, invocationParams.ToArray());

                logger.LogTrace($"Succesfully executed {command.Type}");
            }
            catch (KeyNotFoundException ex)
            {
                throw new InvalidCommandException("Invalid command, could not find required parameters", command.Parameters.Keys, ex);
            }
        }

        private MethodInfo FindMethod(IWhenDoCommandHandler handler, string name, int parameterCount)
        {
            if (!name.EndsWith("Async"))
                name = name + "Async";
            var methods = handler.GetType().GetMethods()
                .Where(m => m.Name.Equals(name));

            foreach (var method in methods)
            {
                var parameters = method.GetParameters();
                if(parameters.Where(x => x.ParameterType.Equals(typeof(IWhenDoMessage))).Count() == 1)
                {
                    if (parameterCount == parameters.Count() - 1)
                        return method;
                }
                else
                {
                    if (parameterCount == parameters.Count())
                        return method;
                }
            }

            return null;
        }
    }
}
