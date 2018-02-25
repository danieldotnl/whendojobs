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

        public WhenDoCommandExecutor(IWhenDoRegistry registry, ILogger<WhenDoCommandExecutor> logger)
        {
            this.registry = registry;
            this.logger = logger;
        }

        public async Task ExecuteAsync(IWhenDoMessageContext context, string type, string methodName, Dictionary<string, object> parameters)
        {
            var commandHandler = registry.GetCommandHandler(type);
            try
            {
                var method = FindMethod(commandHandler, methodName, parameters == null ? 0 : parameters.Count);

                if (method == null)
                    throw new InvalidCommandException($"Could not find method {methodName} in type {type} with {parameters.Count} parameters", parameters.Keys);

                var invocationParams = new List<object>();
                var methodParams = method.GetParameters();

                foreach (var param in methodParams)
                {
                    if (param.ParameterType.Equals(typeof(IWhenDoMessageContext)))
                        invocationParams.Add(context);
                    else
                        invocationParams.Add(parameters[param.Name]);
                }

                await (Task)method.Invoke(commandHandler, invocationParams.ToArray());

                logger.LogTrace($"Succesfully executed {type}");
            }
            catch (KeyNotFoundException ex)
            {
                throw new InvalidCommandException("Invalid command, could not find required parameters", parameters.Keys, ex);
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
                if(parameters.Where(x => x.ParameterType.Equals(typeof(IWhenDoMessageContext))).Count() == 1)
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
