using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhenDoJobs.Core.Exceptions;
using WhenDoJobs.Core.Interfaces;

namespace WhenDoJobs.Core.Services
{
    public class WhenDoExecutor : IWhenDoExecutor
    {
        private IWhenDoRegistry registry;

        public WhenDoExecutor(IWhenDoRegistry registry)
        {
            this.registry = registry;
        }

        public async Task ExecuteJobAsync(IJob job)
        {
            var tasks = new List<Task>();
            foreach (var command in job.Commands)
            {
                await ExecuteCommandAsync(command);
                //switch (command.Execution.Mode)
                //{
                //    case ExecutionMode.Reliable:
                //        break;
                //    case ExecutionMode.Delayed:
                //        break;
                //    case ExecutionMode.Scheduled:
                //        break;
                //    default:
                //        tasks.Add(ExecuteCommandAsync(command));
                //        break;
                //}
            }

            //await Task.WhenAll(tasks);
        }

        public async Task ExecuteCommandAsync(ICommand command)
        {
            var commandHandler = registry.GetCommandHandler(command.Type);
            try
            {
                var method = commandHandler.GetType().GetMethod(command.MethodName);
                var methodParams = method.GetParameters().ToList();

                var invocationParams = new List<object>();
                methodParams.ForEach(x => invocationParams.Add(command.GetParameter(x.Name)));
                await (Task)method.Invoke(commandHandler, invocationParams.ToArray());
            }
            catch (KeyNotFoundException ex)
            {
                throw new InvalidCommandException($"Invalid command, could not find required parameters", command.Parameters.Keys, ex);
            }
        }

    }
}
